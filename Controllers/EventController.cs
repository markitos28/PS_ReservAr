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
        private readonly IAuditLogServices _auditLogService;

        public EventsController(IEventService eventService, IAuditLogServices auditLogService, ILogger<EventsController> logger)
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


        /// <summary>
        /// Busca eventos basados en criterios opcionales como el ID del evento, el nombre, la fecha, el lugar y el estado. Si se proporciona un criterio, se devuelven los eventos que coinciden con ese criterio. Si no se proporcionan criterios, se devuelven todos los eventos disponibles. La respuesta incluye una lista de eventos que cumplen con los criterios de búsqueda, o una lista vacía si no se encuentran coincidencias.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="name"></param>
        /// <param name="eventDate"></param>
        /// <param name="venue"></param>
        /// <param name="status"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] int? eventId,
            [FromQuery] string? name,
            [FromQuery] DateTime? eventDate,
            [FromQuery] string? venue,
            [FromQuery] string? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10 )
        {
            try
            {
                var result = await _eventService.SearchAsync(eventId, name, eventDate, venue, status, pageNumber, pageSize);
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