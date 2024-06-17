using Alee_BookEcommerceAPI.Model.Dto;

namespace Alee_BookEcommerceAPI.Repository.IRepository;

public interface IAuthRepository
{
    Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO);
    Task<UserDTO> Register(RegisterationRequestDTO registerationRequestDTO);
    Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDTO);
    Task RevokeRefreshToken(TokenDTO tokenDTO);
}