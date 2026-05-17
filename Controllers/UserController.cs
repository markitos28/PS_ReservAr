using ReservAr.Services.Interfaces;
using ReservAr.Dtos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ReservAr.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [AllowAnonymous]
    /// <summary>
    /// Controlador para la gestión de perfiles de usuario y registro.
    /// </summary>
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuditLogServices _auditLogService;

        public UserController(IUserService userService, IAuditLogServices auditLogService)
        {
            _userService = userService;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Registra un nuevo usuario en la plataforma.
        /// </summary>
        /// <param name="request">Datos del registro (nombre, email, password).</param>
        /// <returns>Información básica del usuario creado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO request)
        {
            var existingUser = await _userService.GetUserByEmailAsync(request.Email);

            if (existingUser != null)
            {
                // No usamos UserId = 0 porque rompe la FK Audit_Log -> User.
                // Usamos el Id del usuario existente.
                await _auditLogService.Log(
                    existingUser.Id,
                    "REQUEST_USER_REGISTER_ERROR",
                    "User",
                    existingUser.Id.ToString(),
                    "Fallo en el registro: email ya en uso - " + request.Email
                );

                return BadRequest(new { message = "El correo electrónico ya está en uso." });
            }

            var user = await _userService.CreateUserAsync(request.Name, request.Email, request.Password);

            await _auditLogService.Log(
                user.Id,
                "REQUEST_USER_REGISTER_SUCCESS",
                "User",
                user.Id.ToString(),
                "Registro exitoso - " + request.Email
            );

            return Ok(new UserResponse { Id = user.Id, Name = user.Name, Email = user.Email });
        }

        /// <summary>
        /// Busca un usuario por su dirección de correo electrónico.
        /// </summary>
        /// <param name="email">Email del usuario a buscar.</param>
        /// <returns>Datos del usuario si se encuentra.</returns>
        [HttpGet("by-email")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            return Ok(new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            });
        }
    }
}