﻿using DietAssistant.Business;
using DietAssistant.Business.Configuration;
using DietAssistant.Business.Contracts;
using DietAssistant.DataAccess;
using DietAssistant.DataAccess.Contracts;
using DietAssistant.DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;

#pragma warning disable

namespace DietAssistant.WebAPI.Extentions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddConfiguration(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<NutritionApiConfiguration>(
                configuration.GetSection(nameof(NutritionApiConfiguration)));

            services.Configure<AuthConfiguration>(
                configuration.GetSection(nameof(AuthConfiguration)));

            services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<IMealService, MealService>();
            services.AddTransient<IFoodServingService, FoodServingService>();
            services.AddTransient<IFoodCatalogService, FoodCatalogService>();
            services.AddTransient<IDietPlanningService, DietPlanningService>();
            services.AddTransient<IUserStatsService, UserStatsService>();
            services.AddTransient<IProgressLogService, ProgressLogService>();
            services.AddTransient<IGoalService, GoalService>();

            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IUserResolverService, UserResolverService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IMealRepository, MealRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IDietPlanRepository, DietPlanRepository>();
            services.AddTransient<IProgressLogRepository, ProgressLogRepository>();
        }

        public static void AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "DietAssistant.WebAPI",
                    Version = "v1",
                    Description = "OpenAPI schema of DietAssistant REST API",
                    TermsOfService = new Uri("https://tos.com"),
                    Contact = new OpenApiContact() { Email = "lubo99filipov@gmail.com", Name = "Lyubomir Filipov" },
                    License = new OpenApiLicense() { Name = "License of API", Url = new Uri("https://license.com") }
                });
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
                c.MapType<TimeSpan>(() => new OpenApiSchema
                {
                    Type = "string",
                    Example = new OpenApiString("00:00:00")
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration config)
        {
            var authConfig = config.GetSection(nameof(AuthConfiguration)).Get<AuthConfiguration>();

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configuration =>
                {
                    configuration.RequireHttpsMetadata = false;
                    configuration.SaveToken = true;
                    configuration.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(authConfig.SecurityKeyAsBytes),
                        ValidateAudience = true,
                        ValidAudience = authConfig.Audience,
                        ValidateIssuer = true,
                        ValidIssuer = authConfig.Issuer,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
    }
}
