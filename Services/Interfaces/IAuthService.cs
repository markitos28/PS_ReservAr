using ReservAr.Dtos.Auth;

namespace ReservAr.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}