using Alee_BookEcommerceAPI.Data;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Repository.IRepository;

namespace Alee_BookEcommerceAPI.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
        Category = new CategoryRepository(_db);
        Product = new ProductRepository(_db);
        ProductImage = new ProductImageRepository(_db);
        User = new UserRepository(_db);
    }

    public ICategoryRepository Category { get; }
    public IProductRepository Product { get; }
    public IProductImageRepository ProductImage { get; }
    public IUserRepository User { get; }

    public async Task SaveAsync()
    {
        await _db.SaveChangesAsync();
    }
}