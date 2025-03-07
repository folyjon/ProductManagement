using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManagement.Application.Common.Factories;

namespace ProductManagement.Infrastructure.Persistence;

public static class DapperModule
{
    public static IServiceCollection AddDapperInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IQueryHandlerFactory, QueryHandlerFactory>();
        return services;
    }
}