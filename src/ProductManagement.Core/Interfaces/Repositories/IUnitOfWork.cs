namespace ProductManagement.Core.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}