
using DFDSVisitManagementAPI.Domain.src.DTOs.Auth;

namespace DFDSVisitManagementAPI.Business.src.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}