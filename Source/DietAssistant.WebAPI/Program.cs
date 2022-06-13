using DietAssistant.Business.Configuration;
using DietAssistant.DataAccess;
using DietAssistant.Domain;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

var services = builder.Services;
var authConfig = configuration.GetSection(nameof(AuthConfiguration)).Get<AuthConfiguration>();


services.AddDbContext<DietAssistantDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DietAssistantContextConnection")));

services.AddConfiguration(configuration);

services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

services.AddServices();
services.AddRepositories();

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddCustomSwagger();
services.AddJwtConfiguration(authConfig);

var app = builder.Build();

app.UseExceptionHandlerMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DietAssistant.WebAPI v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
