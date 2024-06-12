namespace Alee_BookEcommerceAPI.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    Task SaveAsync();
}