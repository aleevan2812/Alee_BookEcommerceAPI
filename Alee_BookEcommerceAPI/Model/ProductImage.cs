using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alee_BookEcommerceAPI.Model;

public class ProductImage
{
    [Key]
    public int Id { get; set; }

    public string ImageUrl { get; set; }
    public string ImagesLocalPath { get; set; }

    public int ProductId { get; set; }

    [ForeignKey("ProductId")] public Product Product { get; set; }
}