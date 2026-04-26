using ReservAr.Models;
namespace ReservAr.Services.Interfaces
{
    public interface IAuthenticationServices
    {
        string GenerateJwtToken(User user);
    }
}