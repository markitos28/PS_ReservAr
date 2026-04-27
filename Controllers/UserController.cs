using ReservAr.Services;
using ReservAr.Services.Interfaces;
using ReservAr.Dtos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace ReservAr.Controllers
{
    /// <summary>
    /// Controlador para gestionar los usuarios del sistema, incluyendo el registro de nuevos usuarios. Este controlador permite a los usuarios registrarse proporcionando su nombre, correo electrónico y contraseña. Si el correo electrónico ya está en uso, se devuelve un error adecuado. Si el registro es exitoso, se devuelve la información del usuario creado sin incluir la contraseña. Todas las acciones en este controlador permiten acceso sin autenticación, lo que permite a los usuarios registrarse sin necesidad de un token previo.
    /// </summary>
    [ApiController]
    [Route("api/v1/users")]
    [AllowAnonymous] // MNS: Permitir acceso sin autenticación para registrar y loguear usuarios
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuditLogService _auditLogService;

        public UserController(IUserService userService, IAuditLogService auditLogService)
        {
            _userService = userService;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema. Se reciben los datos de registro, incluyendo el nombre, el correo electrónico y la contraseña. Si el correo electrónico ya está en uso, se devuelve un error de "Bad Request" con un mensaje adecuado. Si el registro es exitoso, se devuelve la información del usuario creado (ID, nombre y correo electrónico) sin incluir la contraseña.
        /// </summary>
        /// <param name="request">Datos de registro del usuario</param>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO request)
        {
            var existingUser = await _userService.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _auditLogService.Log(0, "REQUEST_USER_REGISTER_ERROR", "User", "0", "Fallo en el registro: email ya en uso - " + request.Email);
                return BadRequest("Email already in use.");
            }

            var user = await _userService.CreateUserAsync(request.Name, request.Email, request.Password);
            _auditLogService.Log(user.Id, "REQUEST_USER_REGISTER_SUCCESS", "User", user.Id.ToString(), "Registro exitoso - " + request.Email);
            return Ok(new { user.Id, user.Name, user.Email });
        }
    }
}