using Microsoft.EntityFrameworkCore;
using ReservAr.Data;
using ReservAr.Dtos.Auth;
using ReservAr.Services.Interfaces;

namespace ReservAr.Services
{
    public class AuthService : IAuthService
    {
        private readonly ReservArDbContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(ReservArDbContext context, ILogger<AuthService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLower();

            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Email.ToLower() == normalizedEmail);

            if (user == null)
            {
                return null;
            }

            if (user.PasswordHash != request.Password)
            {
                return null;
            }

            try
            {
                return new LoginResponse
                {
                    Token = $"fake-jwt-token-user-{user.Id}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error al autenticar el usuario.");
                throw;
            }
        }
    }
}