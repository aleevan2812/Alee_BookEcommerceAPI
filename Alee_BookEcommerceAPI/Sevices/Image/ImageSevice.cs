using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Repository.IRepository;

namespace Alee_BookEcommerceAPI.Sevices;

public class ImageSevice : IImageService
{
    private readonly IUnitOfWork _unitOfWork;

    public ImageSevice(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateProductImage(IFormFile img, int productId, HttpContext httpContext)
    {
        // Create ProductImage
        ProductImage productImage = new();
        Guid guid = Guid.NewGuid();
        string fileName = "productId" + productId + "-" + guid +
                          Path.GetExtension(img.FileName);
        string filePath = @"wwwroot\ProductImages\" + fileName;

        var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

        using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
        {
            img.CopyTo(fileStream);
        }

        // Lấy URL gốc của ứng dụng bằng cách kết hợp Scheme (HTTP/HTTPS), Host và PathBase từ HttpContext.Request.
        var baseUrl =
            $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}{httpContext.Request.PathBase.Value}";

        // URL đầy đủ của tệp ảnh để sử dụng trên giao diện người dùng.
        productImage.ImageUrl = baseUrl + "/ProductImages/" + fileName;
        // Đường dẫn tệp ảnh lưu trữ trên máy chủ.
        productImage.ImagesLocalPath = filePath;
        productImage.ProductId = productId;

        await _unitOfWork.ProductImage.CreateAsync(productImage);
        await _unitOfWork.SaveAsync();
    }

    public async Task DeleteProductImage(int id)
    {
        var productImage = await _unitOfWork.ProductImage.GetAsync(u => u.Id == id);

        var filePath = productImage.ImagesLocalPath;
        if (File.Exists(filePath)) File.Delete(filePath);
        await _unitOfWork.ProductImage.RemoveAsync(productImage);
        // await _unitOfWork.SaveAsync();
    }
}