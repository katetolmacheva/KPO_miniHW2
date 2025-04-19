using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ZooWebApp.Application.Services;
using ZooWebApp.Domain.Common.Interfaces;
using ZooWebApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5001");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IFeedingScheduleService, FeedingScheduleService>();
builder.Services.AddScoped<IZooStatisticsService, ZooStatisticsService>();
builder.Services.AddSingleton<IAnimalRepository, InMemoryAnimalRepository>();
builder.Services.AddSingleton<IEnclosureRepository, InMemoryEnclosureRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ZooWebApp API V1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
