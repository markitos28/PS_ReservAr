using Microsoft.AspNetCore.Mvc;
using ReservAr.Dtos.Events;
using ReservAr.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ReservAr.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;
        private readonly IAuditLogService _auditLogService;

        public EventsController(IEventService eventService, IAuditLogService auditLogService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
            _auditLogService = auditLogService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            try
            {
                var result = await _eventService.CreateAsync(request);
                _auditLogService.Log(0, "REQUEST_EVENT_CREATE", "Event", result.Id.ToString(), "Evento creado - " + result.Name);
                return CreatedAtAction(nameof(GetById), new { eventId = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _auditLogService.Log(0, "REQUEST_EVENT_CREATE_FAILED", "Event", "0", "Fallo al crear evento: " + request.Name + " - " + ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error inesperado al crear evento.");
                return StatusCode(500, new { message = "Error interno al crear evento.", detail = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("{eventId:int}")]
        public async Task<IActionResult> Update(int eventId, [FromBody] UpdateEventRequest request)
        {
            try
            {
                var result = await _eventService.UpdateAsync(eventId, request);

                if (result == null)
                {
                    _auditLogService.Log(0, "REQUEST_EVENT_UPDATE_FAILED", "Event", eventId.ToString(), "Fallo al actualizar evento: evento no encontrado - ID " + eventId);
                    return NotFound(new { message = "Evento no encontrado." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _auditLogService.Log(0, "REQUEST_EVENT_UPDATE_FAILED", "Event", eventId.ToString(), "Fallo al actualizar evento: " + ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error inesperado al actualizar evento.");
                return StatusCode(500, new { message = "Error interno al actualizar evento.", detail = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet("{eventId:int}")]
        public async Task<IActionResult> GetById(int eventId)
        {
            try
            {
                var result = await _eventService.GetByIdAsync(eventId);

                if (result == null)
                {
                    _auditLogService.Log(0, "REQUEST_EVENT_GET_FAILED", "Event", eventId.ToString(), "Fallo al obtener evento: evento no encontrado - ID " + eventId);
                    return NotFound(new { message = "Evento no encontrado." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error inesperado al obtener evento.");
                return StatusCode(500, new { message = "Error interno al obtener evento.", detail = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] int? eventId,
            [FromQuery] string? name,
            [FromQuery] DateTime? eventDate,
            [FromQuery] string? venue,
            [FromQuery] string? status)
        {
            try
            {
                var result = await _eventService.SearchAsync(eventId, name, eventDate, venue, status);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _auditLogService.Log(0, "REQUEST_EVENT_SEARCH_FAILED", "Event", "0", "Fallo al buscar eventos: " + ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CODE-ERROR] - Error inesperado al buscar eventos.");
                _auditLogService.Log(0, "REQUEST_EVENT_SEARCH_ERROR", "Event", "0", "Error inesperado al buscar eventos: " + ex.Message);
                return StatusCode(500, new { message = "Error interno al buscar eventos.", detail = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}