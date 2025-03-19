using Azure.Storage.Blobs;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using ProductManagement.Api.Health;
using ProductManagement.Api.Options;
using ProductManagement.Application.Behaviour;
using ProductManagement.Application.Features.Products.Queries;
using ProductManagement.Application.Interfaces.Services;
using ProductManagement.Application.Validators;
using ProductManagement.Core.Interfaces.Repositories;
using ProductManagement.Infrastructure.Persistence;
using ProductManagement.Infrastructure.Persistence.Repositories;
using ProductManagement.Infrastructure.Services;

namespace ProductManagement.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Controllers
        services.AddControllers()
            .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

        // CQRS with MediatR
        var assembly = typeof(Program).Assembly;
        //services.AddCarter();
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProductsQueryHandler).Assembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(ProductValidator).Assembly);
        services.AddFluentValidationAutoValidation();

        // Database Configuration
        services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
        services.Configure<StorageContainerSettings>(configuration.GetSection(StorageContainerSettings.SectionName));
        services.Configure<RetryPolicySettings>(configuration.GetSection(RetryPolicySettings.SectionName));

        services.AddDapperInfrastructure(configuration);

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            options.UseSqlServer(databaseSettings.DefaultDb.ConnectionString);
        });

        // Azure Blob Storage
        services.AddSingleton(serviceProvider =>
        {
            var storageSettings = serviceProvider.GetRequiredService<IOptions<StorageContainerSettings>>().Value;
            return new BlobServiceClient(storageSettings.ProductFiles.ConnectionString);
        });
        services.AddSingleton<IStorageService, BlobStorageService>();

        // Repository Pattern
        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }

    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        return services;
    }
}