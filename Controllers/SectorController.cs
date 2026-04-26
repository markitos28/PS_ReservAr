using Microsoft.AspNetCore.Mvc;
using ReservAr.Dtos.Sectors;
using ReservAr.Services.Interfaces;

namespace ReservAr.Controllers
{
    [ApiController]
    [Route("api/v1/sectors")]
    public class SectorsController : ControllerBase
    {
        private readonly ISectorService _sectorService;
        private readonly ILogger<SectorsController> _logger;

        public SectorsController(ISectorService sectorService, ILogger<SectorsController> logger)
        {
            _sectorService = sectorService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSectorRequest request)
        {
            try
            {
                var result = await _sectorService.CreateAsync(request);
                return CreatedAtAction(nameof(GetById), new { sectorId = result.Id }, result);
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

        [HttpPut("{sectorId:int}/price")]
        public async Task<IActionResult> UpdatePrice(int sectorId, [FromBody] UpdateSectorRequest request)
        {
            var result = await _sectorService.UpdatePriceAsync(sectorId, request);

            if (result == null)
            {
                return NotFound(new { message = "Sector no encontrado." });
            }

            return Ok(result);
        }

        [HttpGet("{sectorId:int}")]
        public async Task<IActionResult> GetById(int sectorId)
        {
            var result = await _sectorService.GetByIdAsync(sectorId);

            if (result == null)
            {
                return NotFound(new { message = "Sector no encontrado." });
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] int? eventId, [FromQuery] string? name)
        {
            var result = await _sectorService.SearchAsync(eventId, name);
            return Ok(result);
        }
    }
}