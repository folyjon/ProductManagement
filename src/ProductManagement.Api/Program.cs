using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductManagement.Infrastructure.Persistence;
using ProductManagement.Api.Extensions;
using ProductManagement.Api.Health;
using ProductManagement.Api.Options;

var builder = WebApplication.CreateBuilder(args);

// Load Configuration
var configuration = builder.Configuration;

// Register Services
builder.Services.AddApplicationServices(configuration);
builder.Services.AddApiDocumentation();
builder.Services.AddCustomCors();
builder.Services.AddCustomHealthChecks();

builder.Services.AddScoped<SqlServerHealthCheck>();
builder.Services.AddScoped<AzureBlobStorageHealthCheck>(sp =>
{
    var options = sp.GetRequiredService<IOptionsSnapshot<StorageContainerSettings>>().Value;
    return new AzureBlobStorageHealthCheck(options.ProductFiles); // Correct way to access nested config
});

var app = builder.Build();

// Apply EF Core Migrations Automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductManagement API V1");
        c.RoutePrefix = string.Empty;  // Makes Swagger the default page
    });
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseGlobalExceptionHandler();

app.MapControllers();
app.UseCustomHealthChecks();

app.Run();