namespace ProductManagement.Core.Interfaces.Repositories
{
    public interface IRepository<T>
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
