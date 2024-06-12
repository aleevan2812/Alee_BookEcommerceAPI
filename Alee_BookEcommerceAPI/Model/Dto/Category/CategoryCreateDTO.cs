using System.ComponentModel.DataAnnotations;

namespace Alee_BookEcommerceAPI.Model.Dto;

public class CategoryCreateDTO
{
    [Required] [MaxLength(30)] public string Name { get; set; }

    [Range(1, 100, ErrorMessage = "Display Order must be between 1 - 100")]
    public int DisplayOrder { get; set; }
}