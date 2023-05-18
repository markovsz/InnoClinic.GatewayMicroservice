using AggregatorMicroservice.FilterAttributes;
using AggregatorMicroservice.Services;
using AggregatorMicroservice.Services.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AggregatorMicroservice.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IHttpCrudClient, HttpCrudClient>();
        services.AddScoped<IAggregatorsService, AggregatorsService>();
    }

    public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var identityServerConfig = configuration
                    .GetSection("IdentityServer");

        var scopes = identityServerConfig
                    .GetSection("Scopes");

        services.AddAuthentication(config =>
        config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, config =>
        {
            config.Authority = identityServerConfig
                .GetSection("Address").Value;
            config.Audience = identityServerConfig
                .GetSection("Audience").Value;
            config.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = identityServerConfig
                    .GetSection("Address").Value,
                ValidateIssuer = true,
                ValidAudience = scopes.GetSection("Basic").Value,

                ValidateAudience = true
            };
        });
    }

    public static void ConfigureFilterAttributes(this IServiceCollection services)
    {
        services.AddScoped<ExtractRoleAttribute>();
        services.AddScoped<ExtractJwtTokenAttribute>();
    }
}
