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
        private readonly IAuditLogServices _auditLogService;

        public ReservationsController(IReservationService reservationService, ILogger<ReservationsController> logger, IAuditLogServices auditLogService)
        {
            _reservationService = reservationService;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
        {
            try
            {
                var result = await _reservationService.CreateAsync(request);
                _auditLogService.Log(request.UserId, "REQUEST_RESERVATION_CREATE", "Reservation", request.UserId.ToString(), $"Reserva creada - ReservationId: {result.Id} SeatId: {request.SeatId}");
                return CreatedAtAction(nameof(GetById), new { reservationId = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("[CODE-ERROR] - {Message}", ex.Message);
                _auditLogService.Log(request.UserId, "REQUEST_RESERVATION_CREATE_FAILED", "Reservation", request.UserId.ToString(), $"Fallo al crear reserva: recurso no encontrado - SeatId: {request.SeatId} - {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("[CODE-ERROR] - {Message}", ex.Message);
                _auditLogService.Log(request.UserId, "REQUEST_RESERVATION_CREATE_FAILED", "Reservation", request.UserId.ToString(), $"Fallo al crear reserva: operación inválida - SeatId: {request.SeatId} - {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error inesperado al crear reserva.");
                _auditLogService.Log(request.UserId, "REQUEST_RESERVATION_CREATE_FAILED", "Reservation", request.UserId.ToString(), $"Fallo al crear reserva: error inesperado - SeatId: {request.SeatId} - {ex.Message}");
                return StatusCode(500, new { message = "Error interno al crear reserva.", detail = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("{reservationId:guid}")]
        public async Task<IActionResult> GetById(Guid reservationId)
        {
            var result = await _reservationService.GetByIdAsync(reservationId);

            if (result == null)
            {
                _auditLogService.Log(0, "REQUEST_RESERVATION_GET_FAILED", "Reservation", "0", $"Reserva no encontrada - ReservationId: {reservationId}");
                return NotFound(new { message = "Reserva no encontrada." });
            }
            _auditLogService.Log(0, "REQUEST_RESERVATION_GET", "Reservation", "0", $"Reserva obtenida - ReservationId: {reservationId}");
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
                    _auditLogService.Log(0, "REQUEST_RESERVATION_UPDATE_FAILED", "Reservation", "0", $"Reserva no encontrada - ReservationId: {reservationId}");
                    return NotFound(new { message = "Reserva no encontrada." });
                }
                _auditLogService.Log(0, "REQUEST_RESERVATION_UPDATE", "Reservation", "0", $"Reserva actualizada - ReservationId: {reservationId}, NewStatus: {request.Status}");
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _auditLogService.Log(0, "REQUEST_RESERVATION_UPDATE_FAILED", "Reservation", "0", $"Fallo al actualizar reserva: operación inválida - ReservationId: {reservationId} - {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPatch("expire-pending")]
        public async Task<IActionResult> ExpirePending()
        {
            var expiredCount = await _reservationService.ExpirePendingReservationsAsync();
            _auditLogService.Log(0, "REQUEST_RESERVATION_EXPIRE_PENDING", "Reservation", "0", $"Reservas expiradas - Count: {expiredCount}");
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
            _auditLogService.Log(0, "REQUEST_RESERVATION_SEARCH", "Reservation", "0", $"Búsqueda de reservas - UserId: {userId}, SeatId: {seatId}, Status: {status}");
            return Ok(result);
        }
    }
}