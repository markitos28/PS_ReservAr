using ReservAr.Models;
namespace ReservAr.Services
{
    public interface IAuthenticationServices
    {
        string GenerateJwtToken(User user);
    }
}