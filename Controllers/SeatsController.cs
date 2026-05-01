using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservAr.Dtos.Seats;
using ReservAr.Services.Interfaces;

namespace ReservAr.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/seats")]
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;
        private readonly ILogger<SeatsController> _logger;

        public SeatsController(ISeatService seatService, ILogger<SeatsController> logger)
        {
            _seatService = seatService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSeatRequest request)
        {
            try
            {
                var result = await _seatService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { seatId = result.Id }, result);
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
        }

        [HttpPatch("{seatId:guid}")]
        public async Task<IActionResult> Update(Guid seatId, [FromBody] UpdateSeatRequest request)
        {
            try
            {
                var result = await _seatService.UpdateAsync(seatId, request);

                if (result == null)
                {
                    return NotFound(new { message = "Asiento no encontrado." });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("[CODE-ERROR] - {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{seatId:guid}")]
        public async Task<IActionResult> GetById(Guid seatId)
        {
            var result = await _seatService.GetByIdAsync(seatId);

            if (result == null)
            {
                return NotFound(new { message = "Asiento no encontrado." });
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Search(
            [FromQuery] int? seatNumber,
            [FromQuery] string? rowIdentifier,
            [FromQuery] int? sectorId,
            [FromQuery] string? status,
            [FromQuery] int? version)
        {
            var result = await _seatService.SearchAsync(seatNumber, rowIdentifier, sectorId, status, version);
            return Ok(result);
        }
    }
}