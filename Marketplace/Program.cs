using Marketplace.Api;
using Marketplace.Domain.Repositories;
using Marketplace.Domain.Services;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using Raven.Client.Documents;

var builder = WebApplication.CreateBuilder(args);

// Document store tilf�jes til DI-containeren
var store = new DocumentStore()
{
    Urls = new[] { "http://localhost:8080" },
    Database = "Marketplace_Chapter8",
    Conventions =
    {
        FindIdentityProperty = m => m.Name == "_databaseId"
    }
};
store.Initialize();

builder.Services.AddSingleton<ICurrencyLookup, FixedCurrencyLookup>();
// Document store registreres som en singleton
builder.Services.AddScoped(c => store.OpenAsyncSession());
// Unit of work registreres som en scoped service
builder.Services.AddScoped<IUnitOfWork, RavenDbUnitOfWork>();
builder.Services.AddScoped<IClassifiedAdRepository, ClassifiedAdRepository>();
builder.Services.AddScoped<ClassifiedAdsApplicationService>();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "ClassifiedAds",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();