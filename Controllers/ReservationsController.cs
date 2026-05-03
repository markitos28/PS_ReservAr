using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservAr.Dtos.Reservations;
using ReservAr.Services.Interfaces;

namespace ReservAr.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/reservations")]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(IReservationService reservationService, ILogger<ReservationsController> logger)
        {
            _reservationService = reservationService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
        {
            try
            {
                var result = await _reservationService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { reservationId = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("[CODE-ERROR] - {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("[CODE-ERROR] - {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error inesperado al crear reserva.");
                return StatusCode(500, new { message = "Error interno al crear reserva.", detail = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("{reservationId:guid}")]
        public async Task<IActionResult> GetById(Guid reservationId)
        {
            var result = await _reservationService.GetByIdAsync(reservationId);

            if (result == null)
            {
                return NotFound(new { message = "Reserva no encontrada." });
            }

            return Ok(result);
        }

        [HttpPatch("{reservationId:guid}")]
        public async Task<IActionResult> Update(Guid reservationId, [FromBody] UpdateReservationRequest request)
        {
            try
            {
                var result = await _reservationService.UpdateAsync(reservationId, request);

                if (result == null)
                {
                    return NotFound(new { message = "Reserva no encontrada." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("expire-pending")]
        public async Task<IActionResult> ExpirePending()
        {
            var expiredCount = await _reservationService.ExpirePendingReservationsAsync();

            return Ok(new
            {
                expiredCount = expiredCount
            });
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] int? userId,
            [FromQuery] Guid? seatId,
            [FromQuery] string? status)
        {
            var result = await _reservationService.SearchAsync(userId, seatId, status);
            return Ok(result);
        }
    }
}