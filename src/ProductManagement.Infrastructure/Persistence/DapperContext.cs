using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ProductManagement.Infrastructure.Persistence;

public class DapperContext
{
    private readonly string? _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}