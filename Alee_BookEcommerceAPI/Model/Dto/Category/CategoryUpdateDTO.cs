using System.ComponentModel.DataAnnotations;

namespace Alee_BookEcommerceAPI.Model.Dto;

public class CategoryUpdateDTO
{
    [Required]public int Id { get; set; }
    [Required] public string Name { get; set; }

    [Range(1, 100, ErrorMessage = "Display Order must be between 1 - 100")]
    public int DisplayOrder { get; set; }
}