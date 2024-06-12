using System.Net;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto;
using Alee_BookEcommerceAPI.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Alee_BookEcommerceAPI.Controllers.V1;

[Route("api/v{version:apiVersion}/CategoryAPI")]
[ApiVersion("1.0")]
[ApiController]
public class CategoryAPIController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    protected APIResponse _apiResponse;


    public CategoryAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _apiResponse = new APIResponse();
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<APIResponse>> GetCategories([FromQuery] string? search, int pageSize = 0,
        int pageNumber = 1)
    {
        try
        {
            IEnumerable<Category> categories;
            categories = await _unitOfWork.Category.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

            // use search
            if (!string.IsNullOrEmpty(search))
                categories = categories.Where(u => u.Name.ToLower().Contains(search));

            _apiResponse.Result = _mapper.Map<List<CategoryDTO>>(categories);
            // ;
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


    [HttpGet("{id:int}", Name = "GetCategory")]
    public async Task<ActionResult<APIResponse>> GetCategory(int id)
    {
        try
        {
            if (id == 0)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var categories = await _unitOfWork.Category.GetAsync(u => u.Id == id);

            if (categories == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }

            _apiResponse.Result = _mapper.Map<CategoryDTO>(categories);
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
    public async Task<ActionResult<APIResponse>> CreateCategory([FromForm] CategoryCreateDTO createDto)
    {
        try
        {
            if (await _unitOfWork.Category.GetAsync(u => u.Name.ToLower() == createDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("ErrorMessages", "Category already exists!");
                return BadRequest(ModelState);
            }

            if (createDto == null)
                return BadRequest(createDto);

            Category category = _mapper.Map<Category>(createDto);

            await _unitOfWork.Category.CreateAsync(category);
            await _unitOfWork.SaveAsync();

            _apiResponse.Result = _mapper.Map<CategoryDTO>(category);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            
            return CreatedAtRoute("GetCategory", new { id = category.Id }, _apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }

    [HttpDelete("{id:int}", Name = "DeleteCategory")]
    public async Task<ActionResult<APIResponse>> DeleteCategory(int id)
    {
        try
        {
            if (id == 0) return BadRequest();

            var category = await _unitOfWork.Category.GetAsync(u => u.Id == id);

            if (category == null) return NotFound();

            await _unitOfWork.Category.RemoveAsync(category);
            await _unitOfWork.SaveAsync();
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.Result = _mapper.Map<CategoryDTO>(category);
            return Ok(_apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }

    [HttpPut("{id:int}",Name = "UpdateCategory")]
    public async Task<ActionResult<APIResponse>> UpdateCategory(int id, [FromForm] CategoryUpdateDTO updateDto)
    {
        try
        {
            if (updateDto == null)
                return BadRequest();
            
            if (updateDto.Id != id)
            {
                ModelState.AddModelError("ErrorMessages", "Can not modify Id!");
                return BadRequest(ModelState);
            }
            
            if (await _unitOfWork.Category.GetAsync(u => u.Id == updateDto.Id) == null)
            {
                ModelState.AddModelError("ErrorMessages", "Category is not exists!");
                return BadRequest(ModelState);
            }
            
            var category = _mapper.Map<Category>(updateDto);

            await _unitOfWork.Category.UpdateAsync(category);
            await _unitOfWork.SaveAsync();

            _apiResponse.Result = category;
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return CreatedAtRoute("GetCategory", new { id = category.Id }, _apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }
}