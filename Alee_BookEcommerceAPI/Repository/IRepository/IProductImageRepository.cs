using Alee_BookEcommerceAPI.Model;

namespace Alee_BookEcommerceAPI.Repository.IRepository;

public interface IProductImageRepository : IRepository<ProductImage>
{
    Task UpdateAsync(ProductImage entity);
}