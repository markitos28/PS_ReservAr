using ReservAr.Services.Interfaces;
using ReservAr.Dtos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ReservAr.Controllers
{
    /// <summary>
    /// Controlador para gestionar la autenticación de usuarios, incluyendo el login. Este controlador maneja las operaciones relacionadas con la autenticación, permitiendo a los usuarios iniciar sesión y obtener un token JWT para acceder a las funcionalidades protegidas del sistema. Todas las acciones en este controlador permiten acceso sin autenticación, lo que permite a los usuarios registrarse y loguearse sin necesidad de un token previo.
    /// </summary>
    [ApiController]
    [Route("api/v1/auth")]
    [AllowAnonymous] // MNS: Permitir acceso sin autenticación para registrar y loguear usuarios
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthenticationServices _authenticationServices;
        private readonly IAuditLogServices _auditLogService;

        public AuthController(IUserService userService, IAuthenticationServices authenticationServices, IAuditLogServices auditLogService)
        {
            _userService = userService;
            _authenticationServices = authenticationServices;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Login de usuario. Se reciben las credenciales del usuario (correo electrónico y contraseña) y se valida su autenticidad. Si las credenciales son válidas, se genera un token JWT que el cliente puede usar para autenticarse en futuras solicitudes. Si las credenciales son inválidas, se devuelve un error de "Unauthorized" con un mensaje adecuado.
        /// </summary>
        /// <param name="request">Credenciales del usuario</param>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null)
            {
                _auditLogService.Log(0, "REQUEST_AUTH_USER_NOT_FOUND", "User", "0", "Fallo de login: usuario no encontrado - " + request.Email);
                return Unauthorized("Invalid email or password.");
            }

            var isValid = await _userService.ValidateUserCredentialsAsync(request.Email, request.Password);
            if (!isValid)
            {
                _auditLogService.Log(user.Id, "REQUEST_AUTH_LOGIN_FAILED", "User", user.Id.ToString(), "Fallo de login: credenciales inválidas - " + request.Email);
                return Unauthorized("Invalid email or password.");
            }

            var token = _authenticationServices.GenerateJwtToken(user);
            _auditLogService.Log(user.Id, "REQUEST_AUTH_LOGIN_SUCCESS", "User", user.Id.ToString(), "Login exitoso - " + request.Email);
            return Ok(new
            {
                access_token = token,
                token_type = "Bearer",
                expires_in = 3600
            });
        }
    }
}