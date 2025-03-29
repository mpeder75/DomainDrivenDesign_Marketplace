using EventStore.ClientAPI;
using Marketplace.ClassifiedAd;
using Marketplace.Domain.Services;
using Marketplace.Domain.Shared;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Marketplace.UserProfile;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .CreateLogger();

builder.Host.UseSerilog();

// Load configuration from appsettings.json
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .Build();

// Configure EventStore connection
var esConnection = EventStoreConnection.Create(
    builder.Configuration["eventStore:connectionString"],
    ConnectionSettings.Create().KeepReconnecting(),
    "Marketplace");

// Register services
var store = new EsAggregateStore(esConnection);
var purgomalumClient = new PurgomalumClient();

builder.Services.AddSingleton(esConnection);
builder.Services.AddSingleton<IAggregateStore>(store);
builder.Services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();
builder.Services.AddSingleton(new ClassifiedAdsApplicationService(
    store, new FixedCurrencyLookup()));
builder.Services.AddSingleton(new UserProfileApplicationService(
    store, text => purgomalumClient.CheckForProfanity(text).GetAwaiter().GetResult()));
builder.Services.AddSingleton<IHostedService, EventStoreService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ClassifiedAds",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClassifiedAds v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();