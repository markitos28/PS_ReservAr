using ReservAr.Services.Interfaces;
using ReservAr.Dtos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ReservAr.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuditLogService _auditLogService;

        public UserController(IUserService userService, IAuditLogService auditLogService)
        {
            _userService = userService;
            _auditLogService = auditLogService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO request)
        {
            var existingUser = await _userService.GetUserByEmailAsync(request.Email);

            if (existingUser != null)
            {
                // No usamos UserId = 0 porque rompe la FK Audit_Log -> User.
                // Usamos el Id del usuario existente.
                _auditLogService.Log(
                    existingUser.Id,
                    "REQUEST_USER_REGISTER_ERROR",
                    "User",
                    existingUser.Id.ToString(),
                    "Fallo en el registro: email ya en uso - " + request.Email
                );

                return BadRequest("Email already in use.");
            }

            var user = await _userService.CreateUserAsync(request.Name, request.Email, request.Password);

            _auditLogService.Log(
                user.Id,
                "REQUEST_USER_REGISTER_SUCCESS",
                "User",
                user.Id.ToString(),
                "Registro exitoso - " + request.Email
            );

            return Ok(new { user.Id, user.Name, user.Email });
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            return Ok(new
            {
                user.Id,
                user.Name,
                user.Email
            });
        }
    }
}