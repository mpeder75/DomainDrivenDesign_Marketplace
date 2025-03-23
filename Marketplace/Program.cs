using EventStore.ClientAPI;

using Marketplace.ClassifiedAd;
using Marketplace.Domain.Services;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.UserProfile;
using Raven.Client.Documents;

var builder = WebApplication.CreateBuilder(args);

// Load configuration from appsettings.json
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var store = new DocumentStore
{
    Urls = new[] { "http://localhost:8080" },
    Database = "Marketplace_Chapter8",
    Conventions =
    {
        FindIdentityProperty = x => x.Name == "DbId"
    }
};
store.Initialize();

var purgomalumClient = new PurgomalumClient();

builder.Services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();

builder.Services.AddScoped(c => store.OpenAsyncSession());
builder.Services.AddScoped<IAggregateStore, EsAggregateStore>();
builder.Services.AddScoped<ClassifiedAdsApplicationService>();

builder.Services.AddScoped(c =>
    new UserProfileApplicationService(
        c.GetService<IAggregateStore>(),
        text => purgomalumClient.CheckForProfanity(text).GetAwaiter().GetResult()));


// Configure Event Store connection
var eventStoreConnectionString = builder.Configuration["eventStore:connectionString"];
var connectionSettings = ConnectionSettings.Create().KeepReconnecting();
var eventStoreConnection = EventStoreConnection.Create(eventStoreConnectionString, connectionSettings);

// Ensure the connection is established only once
await eventStoreConnection.ConnectAsync();
builder.Services.AddSingleton(eventStoreConnection);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Contracts",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
