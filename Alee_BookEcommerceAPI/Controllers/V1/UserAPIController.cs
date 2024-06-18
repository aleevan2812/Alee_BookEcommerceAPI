using System.Net;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto;
using Alee_BookEcommerceAPI.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace Alee_BookEcommerceAPI.Controllers.V1;

[Route("api/v{version:apiVersion}/User")]
[ApiVersion("1.0")]
[ApiController]
public class UserAPIController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    protected APIResponse _apiResponse;

    public UserAPIController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _apiResponse = new APIResponse();
    }

    [HttpGet("GetUsers")]
    public async Task<ActionResult<APIResponse>> GetUsers([FromQuery] string? search, int pageSize = 0,
        int pageNumber = 1)
    {
        try
        {
            IEnumerable<ApplicationUser> users;
            users = await _unitOfWork.User.GetAllAsync(pageSize: pageSize, pageNumber: pageNumber);

            // use search
            if (!string.IsNullOrEmpty(search))
                users = users.Where(u => u.Name.ToLower().Contains(search));

            foreach (var user in users)
                user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

            _apiResponse.Result = _mapper.Map<List<UserDTO>>(users);
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

    [HttpGet("GetUser", Name = "GetUser")]
    public async Task<ActionResult<APIResponse>> GetUser([FromQuery] string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var user = await _unitOfWork.User.GetAsync(u => u.Id == id);

            if (user == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }

            user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

            _apiResponse.Result = _mapper.Map<UserDTO>(user);
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

    [HttpPut]
    [Authorize(Roles = SD.Role_Admin)]
    public async Task<ActionResult<APIResponse>> UpdateRoleUser([FromForm] RoleUserUpdateDTO updateDto)
    {
        try
        {
            var user = await _unitOfWork.User.GetAsync(u => u.Id == updateDto.Id);

            if (user == null)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                return NotFound(_apiResponse);
            }

            var oldRole = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

            if (oldRole != null) await _userManager.RemoveFromRoleAsync(user, oldRole);

            if (updateDto.Role != oldRole)
            {
                await _userManager.AddToRoleAsync(user, updateDto.Role);
                await _unitOfWork.User.UpdateAsync(user);
                await _unitOfWork.SaveAsync();
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            return CreatedAtRoute("GetUser", new { id = user.Id }, _apiResponse);
        }
        catch (Exception e)
        {
            _apiResponse.IsSuccess = false;
            _apiResponse.ErrorMessages = new List<string> { e.ToString() };
        }

        return _apiResponse;
    }
}