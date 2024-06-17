namespace Alee_BookEcommerceAPI.Repository.IRepository;

public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    IProductImageRepository ProductImage { get; }
    IUserRepository User { get; }
    Task SaveAsync();
}