﻿using Manual_Ocelot.Configurations;
using Manual_Ocelot.Entities;
using Manual_Ocelot.Middlewares;
using Manual_Ocelot.Services.GatewayServices;
using Manual_Ocelot.Services.TokenValidationServices;
using Microsoft.EntityFrameworkCore;

namespace Manual_Ocelot.Dependencies;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        WebApplicationBuilder builder
    )
    {
        builder
            .Configuration.SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile(
                $"appsettings.{builder.Environment.EnvironmentName}.json",
                optional: false,
                reloadOnChange: true
            )
            .AddEnvironmentVariables();

        builder.Services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

        builder.Services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
        });

        builder.Services.AddSingleton<IGatewayService, GatewayService>();
        builder.Services.AddScoped<ITokenValidationService, TokenValidationService>();
        builder.Services.Configure<AppSetting>(builder.Configuration);
        builder.Services.AddHttpClient();

        return services;
    }

    public static IApplicationBuilder AddApiGateway(this WebApplication app)
    {
        return app.UseMiddleware<GatewayMiddleware>();
    }
}
