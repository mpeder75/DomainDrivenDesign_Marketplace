using EventStore.ClientAPI;
using Marketplace;
using Marketplace.ClassifiedAd;
using Marketplace.Domain.Services;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Marketplace.UserProfile;
using Microsoft.OpenApi.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Configure services
var services = builder.Services;
var configuration = builder.Configuration;
var environment = builder.Environment;

// EventStore setup
var esConnection = EventStoreConnection.Create(
    configuration["eventStore:connectionString"],
    ConnectionSettings.Create().KeepReconnecting(),
    environment.ApplicationName);
var store = new EsAggregateStore(esConnection);
var purgomalumClient = new PurgomalumClient();

// RavenDB setup
var documentStore = ConfigureRavenDb(configuration.GetSection("ravenDb"));
var getSession = () => documentStore.OpenAsyncSession();

services.AddTransient(c => getSession());
services.AddSingleton(esConnection);
services.AddSingleton<IAggregateStore>(store);

// Application services
services.AddSingleton(new ClassifiedAdsApplicationService(
    store, new FixedCurrencyLookup()));
services.AddSingleton(new UserProfileApplicationService(
    store, text => purgomalumClient.CheckForProfanity(text).GetAwaiter().GetResult()));

// Projections
var projectionManager = new ProjectionManager(esConnection,
    new RavenDbCheckpointStore(getSession, "readmodels"),
    new ClassifiedAdDetailsProjection(getSession,
        async userId => (await getSession.GetUserDetails(userId))?.DisplayName),
    new ClassifiedAdUpcasters(esConnection,
        async userId => (await getSession.GetUserDetails(userId))?.PhotoUrl),
    new UserDetailsProjection(getSession));

services.AddSingleton<IHostedService>(
    new EventStoreService(esConnection, projectionManager));

// API and Swagger
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClassifiedAds",
        Version = "v1"
    });
});

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseRouting();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClassifiedAds v1"));

app.MapControllers();

// Start the application
await app.RunAsync();

// Helper method for RavenDB configuration
static IDocumentStore ConfigureRavenDb(IConfiguration configuration)
{
    var store = new DocumentStore
    {
        Urls = new[] { configuration["server"] },
        Database = configuration["database"]
    };
    store.Initialize();
    var record = store.Maintenance.Server.Send(
        new GetDatabaseRecordOperation(store.Database));
    if (record == null)
        store.Maintenance.Server.Send(
            new CreateDatabaseOperation(new DatabaseRecord(store.Database)));

    return store;
}

// Extension method for user details retrieval
public static class SessionExtensions
{
    public static async Task<ReadModels.UserDetails> GetUserDetails(
        this Func<IAsyncDocumentSession> getSession, Guid userId)
    {
        using var session = getSession();
        return await session.LoadAsync<ReadModels.UserDetails>(userId.ToString());
    }
}