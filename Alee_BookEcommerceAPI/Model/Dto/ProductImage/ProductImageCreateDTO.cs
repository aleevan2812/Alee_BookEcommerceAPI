using System.ComponentModel.DataAnnotations.Schema;

namespace Alee_BookEcommerceAPI.Model.Dto.ProductImage;

public class ProductImageCreateDTO
{
    public IFormFile ProductImage { get; set; }
    public int ProductId { get; set; }
}