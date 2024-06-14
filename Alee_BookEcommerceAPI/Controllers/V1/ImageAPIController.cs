using System.Net;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Alee_BookEcommerceAPI.Controllers.V1;

[Route("api/v{version:apiVersion}/ImageAPI")]
[ApiVersion("1.0")]
[ApiController]
public class ImageAPIController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    protected APIResponse _apiResponse;

    public ImageAPIController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _apiResponse = new APIResponse();
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
}