using DietAssistant.Business;
using DietAssistant.DataAccess;
using DietAssistant.WebAPI.Extentions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

var services = builder.Services;

services.AddDbContext<DietAssistantDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DietAssistantContextConnection")));

services.Configure<NutritionApiConfiguration>(
    configuration.GetSection(nameof(NutritionApiConfiguration)));

services.AddTransient<NutritionClient>();

services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandlerMiddleware();

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
