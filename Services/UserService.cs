using ReservAr.Data;
using ReservAr.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ReservAr.Services
{
    public class UserService : IUserService
    {
        private readonly ReservArDbContext _dbContext;

        public UserService(ReservArDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUserAsync(string name, string email, string password)
        {
            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = HashPassword(password) 
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await GetUserByEmailAsync(email);
            if (user == null) return false;
            bool isValid = VerifyPassword(password, user.PasswordHash);
            if (!isValid) return false;
            
            return true;
        }

        private string HashPassword(string password)
        {
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = SHA256.HashData(passwordBytes);
            return Convert.ToBase64String(hashBytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hashedPassword = HashPassword(password);
            if (hashedPassword.Equals(storedHash))
            {
                return true;
            }
            return false;
        }
    }
}