using Alee_BookEcommerceAPI.Model;

namespace Alee_BookEcommerceAPI.Repository.IRepository;

public interface IProductRepository : IRepository<Product>
{
    Task UpdateAsync(Product entity);
}