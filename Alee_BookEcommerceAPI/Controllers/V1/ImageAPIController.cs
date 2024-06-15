using System.Net;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto.ProductImage;
using Alee_BookEcommerceAPI.Repository.IRepository;
using Alee_BookEcommerceAPI.Sevices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Alee_BookEcommerceAPI.Controllers.V1;

[Route("api/v{version:apiVersion}/ImageAPI")]
[ApiVersion("1.0")]
[ApiController]
public class ImageAPIController : ControllerBase
{
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    protected APIResponse _apiResponse;

    public ImageAPIController(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageService = imageService;
        _apiResponse = new APIResponse();
    }

    [HttpGet]
    public async Task<ActionResult<APIResponse>> GetImages([FromQuery] int pageSize = 0,
        int pageNumber = 1)
    {
        try
        {
            IEnumerable<ProductImage> productImages =
                await _unitOfWork.ProductImage.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

            _apiResponse.Result = _mapper.Map<List<ProductImageDTO>>(productImages);
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<APIResponse>> GetImage(int id)
    {
        try
        {
            if (id == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var images = await _unitOfWork.ProductImage.GetAsync(u => u.Id == id);

            if (images == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }

            _apiResponse.Result = _mapper.Map<ProductImageDTO>(images);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<APIResponse>> DeleteImage(int id)
    {
        try
        {
            if (id == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var image = await _unitOfWork.ProductImage.GetAsync(u => u.Id == id);

            if (image == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }

            string filePath = image.ImagesLocalPath;
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            await _unitOfWork.ProductImage.RemoveAsync(image);
            await _unitOfWork.SaveAsync();

            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }

    [HttpPost]
    public async Task<ActionResult<APIResponse>> CreateImage([FromForm] ProductImageCreateDTO createDto)
    {
        try
        {
            // Create ProductImage
            ProductImage productImage = new();
            Guid guid = Guid.NewGuid();
            string fileName = "productId" + createDto.ProductId + "-" + guid +
                              Path.GetExtension(createDto.ProductImage.FileName);
            string filePath = @"wwwroot\ProductImages\" + fileName;

            var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

            using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
            {
                createDto.ProductImage.CopyTo(fileStream);
            }

            // Lấy URL gốc của ứng dụng bằng cách kết hợp Scheme (HTTP/HTTPS), Host và PathBase từ HttpContext.Request.
            var baseUrl =
                $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

            // URL đầy đủ của tệp ảnh để sử dụng trên giao diện người dùng.
            productImage.ImageUrl = baseUrl + "/ProductImages/" + fileName;
            // Đường dẫn tệp ảnh lưu trữ trên máy chủ.
            productImage.ImagesLocalPath = filePath;
            productImage.ProductId = createDto.ProductId;

            await _unitOfWork.ProductImage.CreateAsync(productImage);
            await _unitOfWork.SaveAsync();

            _apiResponse.StatusCode = HttpStatusCode.OK;
            return CreatedAtRoute("GetImage", new { id = productImage.Id }, _apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }
}