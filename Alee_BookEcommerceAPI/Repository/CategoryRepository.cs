using Alee_BookEcommerceAPI.Data;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Repository.IRepository;

namespace Alee_BookEcommerceAPI.Repository;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _db;

    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task UpdateAsync(Category entity)
    {
        _db.Categories.Update(entity);
    }
}