using System.Net;
using Alee_BookEcommerceAPI.Data;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto;
using Alee_BookEcommerceAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace Alee_BookEcommerceAPI.Controllers;

[Route("api/v{version:apiVersion}/UsersAuth")]
[ApiVersionNeutral] // API trung lập
[ApiController]
public class AuthController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuthRepository _authRepo;
    private readonly IUnitOfWork _unitOfWork;
    protected APIResponse _response;

    public AuthController(IAuthRepository authRepo, IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _authRepo = authRepo;
        _unitOfWork = unitOfWork;
        _roleManager = roleManager;
        _userManager = userManager;
        _response = new APIResponse();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
    {
        var tokenDto = await _authRepo.Login(model);
        if (tokenDto == null || string.IsNullOrEmpty(tokenDto.AccessToken))
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username or password is incorrect");
            return BadRequest(_response);
        }

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = tokenDto;
        return Ok(_response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
    {
        bool ifUserNameUnique = await _unitOfWork.User.IsUniqueUserAsync(model.UserName);
        if (!ifUserNameUnique)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username already exists");
            return BadRequest(_response);
        }

        var user = await _authRepo.Register(model);
        if (user == null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Error while registering");
            return BadRequest(_response);
        }

        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        return Ok(_response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> GetNewTokenFromRefreshToken([FromBody] TokenDTO tokenDTO)
    {
        if (ModelState.IsValid)
        {
            var tokenDTOResponse = await _authRepo.RefreshAccessToken(tokenDTO);
            if (tokenDTOResponse == null || string.IsNullOrEmpty(tokenDTOResponse.AccessToken))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Token Invalid");
                return BadRequest(_response);
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = tokenDTOResponse;
            return Ok(_response);
        }

        _response.IsSuccess = false;
        _response.Result = "Invalid Input";
        return BadRequest(_response);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeRefreshToken([FromBody] TokenDTO tokenDTO)
    {
        if (ModelState.IsValid)
        {
            await _authRepo.RevokeRefreshToken(tokenDTO);
            _response.IsSuccess = true;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        _response.IsSuccess = false;
        _response.Result = "Invalid Input";
        return BadRequest(_response);
    }

    [HttpPost("resetAdmin")]
    public async Task<ActionResult<APIResponse>> CreateRole()
    {
        try
        {
            //create roles if they are not created
            if (!(await _roleManager.RoleExistsAsync(SD.Role_Admin)))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Editor));
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Viewer));
            }
            
            //if roles are not created, then we will create admin user as well

            var user = await _unitOfWork.User.GetAsync(u => u.UserName == "admin");
            if (user != null)
            {
                await _unitOfWork.User.RemoveAsync(user);
            }

            await _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin",
                Name = "My name is admin",
                Role = SD.Role_Admin
            }, "Alee123.");
            await _userManager.AddToRoleAsync(user, SD.Role_Admin);

            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = new List<string> { e.ToString() };
        }

        return _response;
    }
}