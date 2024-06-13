using System.ComponentModel.DataAnnotations.Schema;

namespace Alee_BookEcommerceAPI.Model.Dto.ProductImage;

public class ProductImageCreateDTO
{
    public string? ImageUrl { get; set; }
    public IFormFile? Image { get; set; }

    public int ProductId { get; set; }

    [ForeignKey("ProductId")] public Product Product { get; set; }
}