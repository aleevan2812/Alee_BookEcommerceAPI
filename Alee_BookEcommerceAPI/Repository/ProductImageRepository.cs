using Alee_BookEcommerceAPI.Data;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Repository.IRepository;

namespace Alee_BookEcommerceAPI.Repository;

public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    private readonly ApplicationDbContext _db;

    public ProductImageRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task UpdateAsync(ProductImage entity)
    {
        _db.ProductsImages.Update(entity);
    }
}