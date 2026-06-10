using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservAr.Dtos.Reservations;
using ReservAr.Services.Interfaces;

namespace ReservAr.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/reservations")]
    /// <summary>
    /// Controlador para gestionar el ciclo de vida de las reservaciones de asientos.
    /// </summary>
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IAuditLogServices _auditLogService;

        public ReservationsController(IReservationService reservationService, IAuditLogServices auditLogService)
        {
            _reservationService = reservationService;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Crea una nueva reservación para un asiento específico.
        /// </summary>
        /// <param name="request">Datos de la reserva (UserId y SeatId).</param>
        /// <returns>La reserva creada con su estado inicial (PENDIENTE).</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
        {
            try
            {
                var result = await _reservationService.CreateAsync(request);
                await _auditLogService.Log(request.UserId, "REQUEST_RESERVATION_CREATE", "Reservation", request.UserId.ToString(), $"Reserva creada - ReservationId: {result.Id} SeatId: {request.SeatId}");
                return CreatedAtAction(nameof(GetById), new { reservationId = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                await _auditLogService.Log(request.UserId, "REQUEST_RESERVATION_CREATE_FAILED", "Reservation", request.UserId.ToString(), $"Fallo al crear reserva: recurso no encontrado - SeatId: {request.SeatId} - {ex.Message}");
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                await _auditLogService.Log(request.UserId, "REQUEST_RESERVATION_CREATE_FAILED", "Reservation", request.UserId.ToString(), $"Fallo al crear reserva: operación inválida - SeatId: {request.SeatId} - {ex.Message}");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                await _auditLogService.Log(request.UserId, "REQUEST_RESERVATION_CREATE_FAILED", "Reservation", request.UserId.ToString(), $"Fallo al crear reserva: error inesperado - SeatId: {request.SeatId} - {ex.Message}");
                return StatusCode(500, new { message = "Error interno al crear reserva.", detail = ex.InnerException?.Message ?? ex.Message });
            }
        }

        /// <summary>
        /// Obtiene los detalles de una reserva por su identificador único (GUID).
        /// </summary>
        /// <param name="reservationId">ID único de la reserva.</param>
        /// <returns>Detalles de la reserva.</returns>
        [HttpGet("{reservationId:guid}")]
        [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid reservationId)
        {
            var result = await _reservationService.GetByIdAsync(reservationId);

            if (result is null)
            {
                await _auditLogService.Log(-1, "REQUEST_RESERVATION_GET_FAILED", "Reservation", "0", $"Reserva no encontrada - ReservationId: {reservationId}");
                return NotFound(new { message = "Reserva no encontrada." });
            }
            await _auditLogService.Log(-1, "REQUEST_RESERVATION_GET", "Reservation", "0", $"Reserva obtenida - ReservationId: {reservationId}");
            return Ok(result);
        }

        /// <summary>
        /// Actualiza el estado de una reserva.
        /// </summary>
        /// <param name="reservationId">ID de la reserva.</param>
        /// <param name="request">Nuevo estado para la reserva.</param>
        /// <returns>La reserva actualizada.</returns>
        [HttpPatch("{reservationId:guid}")]
        [ProducesResponseType(typeof(ReservationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(Guid reservationId, [FromBody] UpdateReservationRequest request)
        {
            try
            {
                var result = await _reservationService.UpdateAsync(reservationId, request);

                if (result is null)
                {
                    await _auditLogService.Log(-1, "REQUEST_RESERVATION_UPDATE_FAILED", "Reservation", "0", $"Reserva no encontrada - ReservationId: {reservationId}");
                    return NotFound(new { message = "Reserva no encontrada." });
                }
                await _auditLogService.Log(-1, "REQUEST_RESERVATION_UPDATE", "Reservation", "0", $"Reserva actualizada - ReservationId: {reservationId}, NewStatus: {request.Status}");
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                await _auditLogService.Log(-1, "REQUEST_RESERVATION_UPDATE_FAILED", "Reservation", "0", $"Fallo al actualizar reserva: operación inválida - ReservationId: {reservationId} - {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Ejecuta el proceso de expiración para todas las reservas PENDIENTES que hayan superado el tiempo límite.
        /// </summary>
        /// <returns>Cantidad de reservas expiradas durante el proceso.</returns>
        [HttpPatch("expire-pending")]
        [ProducesResponseType( StatusCodes.Status200OK)]
        public async Task<IActionResult> ExpirePending()
        {
            var expiredCount = await _reservationService.ExpirePendingReservationsAsync();
            await _auditLogService.Log(-1, "REQUEST_RESERVATION_EXPIRE_PENDING", "Reservation", "0", $"Reservas expiradas - Count: {expiredCount}");
            return Ok(new
            {
                expiredCount = expiredCount
            });
        }

        /// <summary>
        /// Busca reservas basadas en filtros opcionales como usuario, asiento o estado.
        /// </summary>
        /// <param name="userId">Filtrar por ID de usuario.</param>
        /// <param name="seatId">Filtrar por ID de asiento.</param>
        /// <param name="status">Filtrar por estado (PENDIENTE, PAGADO, etc.).</param>
        /// <returns>Lista de reservas que coinciden con los criterios.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReservationResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(
            [FromQuery] int? userId,
            [FromQuery] Guid? seatId,
            [FromQuery] string? status)
        {
            var result = await _reservationService.SearchAsync(userId, seatId, status);
            await _auditLogService.Log(-1, "REQUEST_RESERVATION_SEARCH", "Reservation", "0", $"Búsqueda de reservas - UserId: {userId}, SeatId: {seatId}, Status: {status}");
            return Ok(result);
        }
    }
}