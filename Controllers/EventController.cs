using Microsoft.AspNetCore.Mvc;
using ReservAr.Dtos.Events;
using ReservAr.Services.Interfaces;

namespace ReservAr.Controllers
{
    [ApiController]
    [Route("api/v1/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            try
            {
                var result = await _eventService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { eventId = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("[CODE-ERROR] - {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
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
                    return NotFound(new { message = "Evento no encontrado." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("[CODE-ERROR] - {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("{eventId:int}")]
        public async Task<IActionResult> GetById(int eventId)
        {
            var result = await _eventService.GetByIdAsync(eventId);

            if (result == null)
            {
                return NotFound(new { message = "Evento no encontrado." });
            }

            return Ok(result);
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
                _logger.LogWarning("[CODE-ERROR] - {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}