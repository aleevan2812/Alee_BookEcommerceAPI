using System.Net;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto;
using Alee_BookEcommerceAPI.Repository.IRepository;
using Alee_BookEcommerceAPI.Sevices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace Alee_BookEcommerceAPI.Controllers.V1;

[Route("api/v{version:apiVersion}/ProductAPI")]
[ApiVersion("1.0")]
[ApiController]
public class ProductAPIController : ControllerBase
{
    private readonly IImageService _imageService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    protected APIResponse _apiResponse;


    public ProductAPIController(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _imageService = imageService;
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
            IEnumerable<Product> products = await _unitOfWork.Product.GetAllAsyncWithProductImages(
                includeProperties: "Category",
                pageSize: pageSize, pageNumber: pageNumber);

            // use search
            if (!string.IsNullOrEmpty(search))
                products = products.Where(u => u.Title.ToLower().Contains(search));

            _apiResponse.Result = _mapper.Map<List<ProductDTO>>(products);
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

            var product = await _unitOfWork.Product.GetAsyncWithProductImages(u => u.Id == id);

            if (product == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }

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
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Editor)]
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
                for (int i = 0; i < createDto.ProductImages.Count(); i++)
                    await _imageService.CreateProductImage(createDto.ProductImages[i], product.Id, HttpContext);

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

    [HttpPut("{id:int}")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Editor)]
    public async Task<ActionResult<APIResponse>> UpdateProduct(int id, [FromForm] ProductUpdateDTO updateDto)
    {
        try
        {
            if (id == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            if (updateDto == null)
                return BadRequest();

            if (updateDto.Id != id)
            {
                ModelState.AddModelError("ErrorMessages", "Can not modify Id!");
                return BadRequest(ModelState);
            }

            // Product from Db
            var product = await _unitOfWork.Product.GetAsyncWithProductImages(u => u.Id == id, false);

            if (product == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }

            // Delete Image
            var productImages = await _unitOfWork.ProductImage.GetAllAsync(u => u.ProductId == id);

            foreach (var productImage in productImages) await _imageService.DeleteProductImage(productImage.Id);

            product = _mapper.Map<Product>(updateDto);

            if (updateDto.ProductImages != null)
            {
                for (int i = 0; i < updateDto.ProductImages.Count(); i++)
                    await _imageService.CreateProductImage(updateDto.ProductImages[i], product.Id, HttpContext);
            }
            
            await _unitOfWork.Product.UpdateAsync(product);
            await _unitOfWork.SaveAsync();

            _apiResponse.Result = product;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return CreatedAtRoute("GetProduct", new { id = product.Id }, _apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }

    [HttpDelete]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Editor)]
    public async Task<ActionResult<APIResponse>> DeleteProduct(int id)
    {
        try
        {
            if (id == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var product = await _unitOfWork.Product.GetAsyncWithProductImages(u => u.Id == id);

            if (product == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }

            // Delete Image
            if (product.ProductImages != null)
                foreach (var productImage in product.ProductImages)
                {
                    var filePath = productImage.ImagesLocalPath;
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    await _unitOfWork.ProductImage.RemoveAsync(productImage);
                }

            await _unitOfWork.Product.RemoveAsync(product);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }
}