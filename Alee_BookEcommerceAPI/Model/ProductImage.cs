using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Alee_BookEcommerceAPI.Model;

public class ProductImage
{
    [Key]
    public int Id { get; set; }

    public string ImageUrl { get; set; }
    public string ImagesLocalPath { get; set; }

    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public Product Product { get; set; }
}