using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ReservAr.Models;

namespace ReservAr.Services
{
    
    public class AuthenticationServices: IAuthenticationServices
    {
        private readonly string _keyJWT;
        // MNS: Este servicio se encarga de la autenticacion de usuarios, manejo de tokens JWT, etc.
        // MNS: Se puede expandir para incluir funcionalidades como refresco de tokens, revocacion, etc.
        public AuthenticationServices(IConfiguration config)
        {
            _keyJWT = config["Jwt:key"] ?? throw new InvalidOperationException("Jwt:key no está configurado");
        }

        public string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var bytes_key = Encoding.UTF8.GetBytes(_keyJWT);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(bytes_key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
