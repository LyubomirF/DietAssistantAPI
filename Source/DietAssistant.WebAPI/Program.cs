using DietAssistant.DataAccess;
using DietAssistant.Domain;
using DietAssistant.WebAPI.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.AddAppConfiguration();

var configuration = builder.Configuration;
var services = builder.Services;

services.AddDbContext<DietAssistantDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DietAssistantContextConnection")));

services.AddConfiguration(configuration);

services.AddHttpContextAccessor();
services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

services.AddServices();
services.AddRepositories();

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddCustomSwagger();
services.AddJwtConfiguration(configuration);

var app = builder.Build();
await app.InitializeApplication();

app.UseExceptionHandlerMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DietAssistant.WebAPI v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();