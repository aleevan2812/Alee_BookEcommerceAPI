using System.Net;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto;
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

            var products = await _unitOfWork.Product.GetAsync(u => u.Id == id);

            if (products == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }

            _apiResponse.Result = _mapper.Map<ProductDTO>(products);
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
    public async Task<ActionResult<APIResponse>> CreateProduct([FromForm] ProductDTO createDto)
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
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }
}