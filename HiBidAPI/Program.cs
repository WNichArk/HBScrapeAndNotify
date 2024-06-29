using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using HiBidAPI.Models.HiBid;
using HiBidAPI.Models.Repositories;
using HiBidAPI.Models.Utility;
using HiBidAPI.Services;
using HiBidAPI.Services.Interfaces;
using LiteDB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICommService, TwilioService>();
builder.Services.AddHttpClient();
builder.Services.AddTransient<HiBidScraper>();
builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();
builder.Services.AddScoped<ICommunicationRepository, CommunicationRepository>();
builder.Services.AddScoped<IHiBidAuctionItemRepository, HiBidAuctionItemRepository>();

//initialize db, inject, get searches for scheduler
var db = new LiteDatabase("HiBid.db");
builder.Services.AddSingleton<ILiteDatabase>(provider => db);
var searchColl = db.GetCollection<HiBidSearch>("searches");
searchColl.EnsureIndex(x => new {x.SearchTerm, x.Radius}, true);
var searches = searchColl.FindAll().ToList();

builder.Services.AddSingleton<IHostedService>(provider =>
{
    var serviceProvider = provider.GetRequiredService<IServiceProvider>();
    return new HiBidSchedulerService(serviceProvider, searches);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
