using DietAssistant.Business;
using DietAssistant.Business.Configuration;
using DietAssistant.Business.Contracts;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace DietAssistant.WebAPI.Extentions
{
    public static class ServiveCollectionExtentions
    {
        public static void AddConfiguration(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<NutritionApiConfiguration>(
                configuration.GetSection(nameof(NutritionApiConfiguration)));

            services.Configure<AuthConfiguration>(
                configuration.GetSection(nameof(AuthConfiguration)));
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IMealLogService, MealLogService>();
            services.AddTransient<IFoodCatalogService, FoodCatalogService>();
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IMealRepository, MealRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
        }

        public static void AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DietAssistant.WebAPI", Version = "v1" });
                c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Description = "Please enter JWT with Bearer into field"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void AddJwtConfiguration(this IServiceCollection services, AuthConfiguration config)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configuration =>
                {
                    configuration.RequireHttpsMetadata = false;
                    configuration.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(config.SecurityKeyAsBytes),
                        ValidateAudience = true,
                        ValidAudience = config.Audience,
                        ValidateIssuer = true,
                        ValidIssuer = config.Issuer,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
    }
}
