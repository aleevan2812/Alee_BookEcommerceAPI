using System.Net;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto;
using Alee_BookEcommerceAPI.Model.Dto.ProductImage;
using Alee_BookEcommerceAPI.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Alee_BookEcommerceAPI.Controllers.V1;

[Route("api/v{version:apiVersion}/ProductAPI")]
[ApiVersion("1.0")]
[ApiController]
public class ProductAPIController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    protected APIResponse _apiResponse;


    public ProductAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _apiResponse = new APIResponse();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetProducts([FromQuery] string? search, int pageSize = 0,
        int pageNumber = 1)
    {
        try
        {
            IEnumerable<Product> products = await _unitOfWork.Product.GetAllAsync(includeProperties: "Category",
                pageSize: pageSize, pageNumber: pageNumber);

            // use search
            if (!string.IsNullOrEmpty(search))
                products = products.Where(u => u.Title.ToLower().Contains(search));
            
            var productsDTO = _mapper.Map<List<ProductDTO>>(products);
            
            IEnumerable<ProductImage> productImages = await _unitOfWork.ProductImage.GetAllAsync(includeProperties: "Product");

            foreach (var image in productImages)
            {
                var product = productsDTO.FirstOrDefault(u => u.Id == image.ProductId);
                if ( product != null)
                {
                    product.ProductImages.Add(new ProductImageDTO()
                    {
                        Id = image.Id,
                        ImageUrl = image.ImageUrl
                    });
                }
            }

            _apiResponse.Result = productsDTO;
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

    [HttpGet("{id:int}", Name = "GetProduct")]
    public async Task<ActionResult<APIResponse>> GetProduct(int id)
    {
        try
        {
            if (id == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var product = await _unitOfWork.Product.GetAsync(u => u.Id == id);

            if (product == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }
            
            var productImages = await _unitOfWork.ProductImage.GetAllAsync(u => u.ProductId == product.Id);

            if (productImages != null) product.ProductImages = productImages;

            _apiResponse.Result = _mapper.Map<ProductDTO>(product);
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
    public async Task<ActionResult<APIResponse>> CreateProduct([FromForm] ProductCreateDTO createDto)
    {
        try
        {
            if (await _unitOfWork.Product.GetAsync(u => u.ISBN.ToLower() == createDto.ISBN.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorMessages", "Product  already exists!");
                return BadRequest(ModelState);
            }

            if (createDto == null)
                return BadRequest(createDto);

            Product product = _mapper.Map<Product>(createDto);

            await _unitOfWork.Product.CreateAsync(product);
            await _unitOfWork.SaveAsync();
            if (createDto.ProductImages != null)
            {
                // product.ProductImages = new List<ProductImage>();

                for (int i = 0; i < createDto.ProductImages.Count(); i++)
                {
                    string fileName = "productId" + product.Id + "-" + i +
                                      Path.GetExtension(createDto.ProductImages[i].FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;

                    var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                    using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                    {
                        createDto.ProductImages[i].CopyTo(fileStream);
                    }

                    // Lấy URL gốc của ứng dụng bằng cách kết hợp Scheme (HTTP/HTTPS), Host và PathBase từ HttpContext.Request.
                    var baseUrl =
                        $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                    // Create ProductImage
                    ProductImage productImage = new();
                    // URL đầy đủ của tệp ảnh để sử dụng trên giao diện người dùng.
                    productImage.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    // Đường dẫn tệp ảnh lưu trữ trên máy chủ.
                    productImage.ImagesLocalPath = filePath;
                    productImage.ProductId = product.Id;

                    await _unitOfWork.ProductImage.CreateAsync(productImage);
                    await _unitOfWork.SaveAsync();

                    // product.ProductImages.Add(productImage); duplicate 2 images
                }

                await _unitOfWork.Product.UpdateAsync(product);
                await _unitOfWork.SaveAsync();
            }

            _apiResponse.Result = _mapper.Map<ProductDTO>(product);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetProduct", new { id = product.Id }, _apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }

    [HttpPut]
    public async Task<ActionResult<APIResponse>> UpdateProduct([FromForm] ProductUpdateDTO updateDto)
    {
        try
        {
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }
}