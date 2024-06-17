using Alee_BookEcommerceAPI.Data;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Alee_BookEcommerceAPI.Repository;

public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task UpdateAsync(ApplicationUser entity)
    { 
        _db.ApplicationUsers.Update(entity);
    }

    public async Task<bool> IsUniqueUserAsync(string username)
    {
        var user = await _db.ApplicationUsers.FirstOrDefaultAsync(x => x.UserName == username);
        if (user == null) return true;
        return false;
    }
}