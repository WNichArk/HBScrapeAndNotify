using HiBidAPI.Models.HiBid;
using HiBidAPI.Services;
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
