using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ProductManagement.Application.Common.Factories;

namespace ProductManagement.Infrastructure.Persistence;

public class QueryHandlerFactory : IQueryHandlerFactory
{
    private readonly string? _connectionString;

    public QueryHandlerFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}