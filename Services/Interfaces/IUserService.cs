using ReservAr.Models;

namespace ReservAr.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(string name, string email, string password);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> ValidateUserCredentialsAsync(string email, string password);
    }
}