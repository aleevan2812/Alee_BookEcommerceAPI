using Alee_BookEcommerceAPI.Data;
using Alee_BookEcommerceAPI.Model;

namespace Alee_BookEcommerceAPI.Repository.IRepository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task UpdateAsync(Product entity)
    {
        _db.Products.Update(entity);
    }
}