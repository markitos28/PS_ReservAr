using ReservAr.Services;
using ReservAr.Services.Interfaces;
using ReservAr.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ReservAr.Controllers
{
    [ApiController]
    [Route("api/User")]
    [AllowAnonymous] // MNS: Permitir acceso sin autenticación para registrar y loguear usuarios
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationServices _authenticationServices;

        public UserController(IUserService userService, IAuthenticationServices authenticationServices)
        {
            _userService = userService;
            _authenticationServices = authenticationServices;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <param name="request">Datos de registro del usuario</param>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO request)
        {
            var existingUser = await _userService.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already in use.");
            }

            var user = await _userService.CreateUserAsync(request.Name, request.Email, request.Password);
            return Ok(new { user.Id, user.Name, user.Email });
        }

        /// <summary>
        /// Login de usuario
        /// </summary>
        /// <param name="request">Credenciales del usuario</param>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var isValid = await _userService.ValidateUserCredentialsAsync(request.Email, request.Password);
            if (!isValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = _authenticationServices.GenerateJwtToken(user);
            return Ok(new
            {
                access_token = token,
                token_type = "Bearer",
                expires_in = 3600
            });
        }
    }
}