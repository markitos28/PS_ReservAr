using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservAr.Dtos.Seats;
using ReservAr.Services.Interfaces;

namespace ReservAr.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/seats")]
    /// <summary>
    /// Controlador para la gestión y consulta de asientos en los sectores.
    /// </summary>
    public class SeatsController : ControllerBase
    {
        private readonly ISeatService _seatService;
        private readonly ILogger<SeatsController> _logger;

        public SeatsController(ISeatService seatService, ILogger<SeatsController> logger)
        {
            _seatService = seatService;
            _logger = logger;
        }

        /// <summary>
        /// Crea un nuevo asiento dentro de un sector.
        /// </summary>
        /// <param name="request">Datos del asiento (Número, fila, ID de sector).</param>
        /// <returns>El asiento creado.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

        /// <summary>
        /// Actualiza parcialmente los datos o el estado de un asiento.
        /// </summary>
        /// <param name="seatId">ID del asiento.</param>
        /// <param name="request">Datos a actualizar.</param>
        /// <returns>El asiento actualizado.</returns>
        [HttpPatch("{seatId:guid}")]
        [ProducesResponseType( typeof(SeatResponse),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Obtiene un asiento específico por su ID.
        /// </summary>
        /// <param name="seatId">ID del asiento.</param>
        /// <returns>Detalles del asiento.</returns>
        [HttpGet("{seatId:guid}")]
        [ProducesResponseType(typeof(SeatResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid seatId)
        {
            var result = await _seatService.GetByIdAsync(seatId);

            if (result == null)
            {
                return NotFound(new { message = "Asiento no encontrado." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Busca asientos según diversos criterios como sector, número, fila o estado.
        /// </summary>
        /// <param name="seatNumber">Número del asiento.</param>
        /// <param name="rowIdentifier">Identificador de la fila.</param>
        /// <param name="sectorId">ID del sector al que pertenece.</param>
        /// <param name="status">Estado actual (DISPONIBLE, RESERVADO, VENDIDO).</param>
        /// <param name="version">Versión para control de concurrencia.</param>
        /// <returns>Lista de asientos que cumplen los criterios.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SeatResponse>), StatusCodes.Status200OK)]
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