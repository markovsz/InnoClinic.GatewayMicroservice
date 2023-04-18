using AggregatorMicroservice.Services;
using AggregatorMicroservice.Services.Abstractions;

namespace AggregatorMicroservice.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IHttpCrudClient, HttpCrudClient>();
        services.AddScoped<IAggregatorsService, AggregatorsService>();
    }
}
