using System.Data;

namespace ProductManagement.Application.Common.Factories;

public interface IQueryHandlerFactory
{
    IDbConnection CreateConnection();
}