namespace Alee_BookEcommerceAPI.Sevices;

public interface IImageService
{
    Task CreateProductImage(IFormFile img, int productId, HttpContext httpContext);

    Task DeleteProductImage(int id);
}