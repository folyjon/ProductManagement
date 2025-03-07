using Azure.Storage.Blobs;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ProductManagement.Api.Extensions;
using ProductManagement.Api.Options;
using ProductManagement.Application.Behaviour;
using ProductManagement.Application.Features.Products.Queries;
using ProductManagement.Application.Interfaces.Services;
using ProductManagement.Application.Validators;
using ProductManagement.Core.Interfaces.Repositories;
using ProductManagement.Infrastructure.Persistence;
using ProductManagement.Infrastructure.Persistence.Repositories;
using ProductManagement.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration
var configuration = builder.Configuration;

// Add Controllers
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Use FluentValidation for ModelState validation
        options.SuppressModelStateInvalidFilter = true;
    });

// Enable MediatR (CQRS Pattern)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProductsQueryHandler).Assembly));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddDapperInfrastructure(builder.Configuration);

// Add FluentValidation and Auto-Register Validators
builder.Services.AddValidatorsFromAssembly(typeof(ProductValidator).Assembly);
builder.Services.AddFluentValidationAutoValidation(); 

// Bind DatabaseSettings
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.SectionName));

// Bind AzureBlobStorageSettings
builder.Services.Configure<StorageContainerSettings>(builder.Configuration.GetSection(StorageContainerSettings.SectionName));

// Bind Retry Policy
builder.Services.Configure<RetryPolicySettings>(builder.Configuration.GetSection(RetryPolicySettings.SectionName));

// Add AppDbContext
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var connectionString = databaseSettings.GetDatabase(DatabaseType.DefaultDb).ConnectionString;

    options.UseSqlServer(connectionString);
});

// Register Azure Blob Storage
//builder.Services.Configure<AzureBlobStorageOptions>(configuration.GetSection(AzureBlobStorageOptions.SectionName));
builder.Services.AddSingleton(new BlobServiceClient(configuration["AzureBlobStorage:ConnectionString"]));
builder.Services.AddSingleton<IStorageService, BlobStorageService>();

// 
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddDapperInfrastructure(configuration);

// Enable Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Management API", Version = "v1" });

    // Add support for JWT Bearer Auth (if needed)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token in the format: Bearer {your_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Enable CORS (if required)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// Apply EF Core Migrations Automatically (Optional)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();  // Ensure database is up to date
}

// Enable Swagger in Development Mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductManagement API V1");
        c.RoutePrefix = string.Empty;  // 👈 This makes Swagger the default page!
    });
}

// Enable CORS
app.UseCors("AllowAll");

app.UseGlobalExceptionHandler();

// Enable Authorization & Authentication (if using JWT)
app.UseAuthorization();
app.UseAuthentication();

// Map Controllers
app.MapControllers();

app.Run();
