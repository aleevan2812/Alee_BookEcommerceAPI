using Alee_BookEcommerceAPI.Model;

namespace Alee_BookEcommerceAPI.Repository.IRepository;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task UpdateAsync(ApplicationUser entity);
    Task<bool> IsUniqueUserAsync(string username);
}