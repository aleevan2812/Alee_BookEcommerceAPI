using Alee_BookEcommerceAPI.Model;

namespace Alee_BookEcommerceAPI.Sevices;

public interface IImageService
{
    Task<ProductImage> CreateProductImage(IFormFile img, int productId, HttpContext httpContext);
}