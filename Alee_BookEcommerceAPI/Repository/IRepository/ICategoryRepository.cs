using Alee_BookEcommerceAPI.Model;

namespace Alee_BookEcommerceAPI.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    Task UpdateAsync(Category entity);
}