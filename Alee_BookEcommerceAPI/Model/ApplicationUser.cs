using Microsoft.AspNetCore.Identity;

namespace Alee_BookEcommerceAPI.Model;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
}