using System.ComponentModel.DataAnnotations;
using Alee_BookEcommerceAPI.Model.Dto.ProductImage;

namespace Alee_BookEcommerceAPI.Model.Dto;

public class ProductDTO
{
    public int Id { get; set; }
    public int CategoryId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string Author { get; set; }

    public string ISBN { get; set; }

    [Display(Name = "List Price")]
    [Range(1, 1000)]
    public double ListPrice { get; set; }

    [Display(Name = "Price for 1-50")]
    [Range(1, 1000)]
    public double Price { get; set; }

    [Display(Name = "Price for 50+")]
    [Range(1, 1000)]
    public double Price50 { get; set; }

    [Display(Name = "Price for 100+")]
    [Range(1, 1000)]
    public double Price100 { get; set; }

    public List<ProductImageDTO>? ProductImages { get; set; } = new List<ProductImageDTO>();
}