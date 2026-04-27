using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ReservAr.Services.Interfaces;
using System.Text;
using ReservAr.Models;

namespace ReservAr.Services
{
    /// <summary>
    /// Servicio de autenticación que se encarga de generar tokens JWT para los usuarios autenticados.
    /// </summary>
    public class AuthenticationServices: IAuthenticationServices
    {
        private readonly string _keyJWT;
        /// <summary>
        /// Constructor que recibe la configuración de la aplicación para obtener la clave secreta utilizada en la generación de tokens JWT.
        /// </summary>
        /// <param name="config"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public AuthenticationServices(IConfiguration config)
        {
            _keyJWT = config["Jwt:key"] ?? throw new InvalidOperationException("Jwt:key no está configurado");
        }

        /// <summary>
        /// Genera un token JWT para el usuario especificado.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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
