using Microsoft.AspNetCore.Mvc;
using ReservAr.Dtos.Users;
using ReservAr.Services.Interfaces;

namespace ReservAr.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            try
            {
                var result = await _userService.CreateAsync(request);
                return Created(string.Empty, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("[CODE-ERROR] - {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetByEmail([FromQuery] string email)
        {
            var result = await _userService.GetByEmailAsync(email);

            if (result == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            return Ok(result);
        }
    }
}