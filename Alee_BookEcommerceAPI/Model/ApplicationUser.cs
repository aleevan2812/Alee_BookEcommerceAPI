using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Alee_BookEcommerceAPI.Model;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; }
    
    [NotMapped]
    public string? Role { get; set; }
}