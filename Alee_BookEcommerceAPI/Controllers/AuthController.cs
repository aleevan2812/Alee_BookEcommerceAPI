using System.Net;
using Alee_BookEcommerceAPI.Model;
using Alee_BookEcommerceAPI.Model.Dto;
using Alee_BookEcommerceAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Alee_BookEcommerceAPI.Controllers;

[Route("api/v{version:apiVersion}/UsersAuth")]
[ApiVersionNeutral] // API trung lập
[ApiController]
public class AuthController : Controller
{
    private readonly IAuthRepository _authRepo;
    private readonly IUnitOfWork _unitOfWork;
    protected APIResponse _response;

    public AuthController(IAuthRepository authRepo, IUnitOfWork unitOfWork)
    {
        _authRepo = authRepo;
        _unitOfWork = unitOfWork;
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
}