using System.ComponentModel.DataAnnotations;

namespace Alee_BookEcommerceAPI.Model.Dto;

public class ProductUpdateDTO
{
    [Required] public string Id { get; set; }

    [Required] public string Title { get; set; }

    public string Description { get; set; }

    [Required] public string Author { get; set; }

    [Required] public string ISBN { get; set; }

    [Required]
    [Display(Name = "List Price")]
    [Range(1, 1000)]
    public double ListPrice { get; set; }

    [Required]
    [Display(Name = "Price for 1-50")]
    [Range(1, 1000)]
    public double Price { get; set; }

    [Display(Name = "Price for 50+")]
    [Range(1, 1000)]
    public double Price50 { get; set; }

    [Display(Name = "Price for 100+")]
    [Range(1, 1000)]
    public double Price100 { get; set; }

    [Required] public int CategoryId { get; set; }

    public List<Model.ProductImage> ProductImages { get; set; }
}