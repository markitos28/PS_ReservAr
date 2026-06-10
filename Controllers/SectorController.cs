using Microsoft.AspNetCore.Mvc;
using ReservAr.Dtos.Sectors;
using ReservAr.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ReservAr.Controllers
{
    /// <summary>
    /// Controlador para gestionar los sectores de un evento, incluyendo creación, actualización de precios y búsqueda.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/v1/sectors")]
    public class SectorsController : ControllerBase
    {
        private readonly ISectorService _sectorService;
        private readonly IAuditLogServices _auditLogService;

        public SectorsController(ISectorService sectorService, IAuditLogServices auditLogService)
        {
            _sectorService = sectorService;
            _auditLogService = auditLogService;
        }

        /// <summary>
        /// Crea un nuevo sector para un evento específico. El sector se asocia a un evento mediante su ID y se le asigna un nombre y un precio. Si el evento no existe o si ya existe un sector con el mismo nombre para ese evento, se devuelve un error adecuado.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateSectorRequest request)
        {
            try
            {
                var result = await _sectorService.CreateAsync(request);
                await _auditLogService.Log(-1, "REQUEST_SECTOR_CREATE_SUCCESS", "Sector", result.Id.ToString(), "Sector creado - " + result.Name);
                return CreatedAtAction(nameof(GetById), new { sectorId = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                await _auditLogService.Log(-1, "REQUEST_SECTOR_CREATE_FAILED", "Sector", "0", "Fallo al crear sector: evento no encontrado - " + ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                await _auditLogService.Log(-1, "REQUEST_SECTOR_CREATE_FAILED", "Sector", "0", "Fallo al crear sector: " + ex.Message);
                return Conflict(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza el precio de un sector existente. Se identifica el sector mediante su ID y se actualiza su precio con el nuevo valor proporcionado. Si el sector no existe, se devuelve un error de "No encontrado". Si el nuevo precio es inválido (por ejemplo, negativo), se devuelve un error de conflicto con un mensaje adecuado.
        /// </summary>
        /// <param name="sectorId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{sectorId:int}/price")]
        [ProducesResponseType(typeof(SectorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePrice(int sectorId, [FromBody] UpdateSectorRequest request)
        {
            var result = await _sectorService.UpdatePriceAsync(sectorId, request);

            if (result is null)
            {
                await _auditLogService.Log(-1, "REQUEST_SECTOR_UPDATE_PRICE_FAILED", "Sector", sectorId.ToString(), "Fallo al actualizar precio de sector: sector no encontrado - ID " + sectorId);
                return NotFound(new { message = "Sector no encontrado." });
            }

            await _auditLogService.Log(-1, "REQUEST_SECTOR_UPDATE_PRICE_SUCCESS", "Sector", sectorId.ToString(), "Precio de sector actualizado - ID " + sectorId);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene los detalles de un sector específico mediante su ID. Si el sector existe, se devuelve su información, incluyendo el nombre, el precio y el ID del evento al que pertenece. Si el sector no existe, se devuelve un error de "No encontrado" con un mensaje adecuado.
        /// </summary>
        /// <param name="sectorId"></param>
        /// <returns></returns>
        [HttpGet("{sectorId:int}")]
        [ProducesResponseType(typeof(SectorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int sectorId)
        {
            var result = await _sectorService.GetByIdAsync(sectorId);

            if (result is null)
            {
                await _auditLogService.Log(-1, "REQUEST_SECTOR_GET_FAILED", "Sector", sectorId.ToString(), "Fallo al obtener sector: sector no encontrado - ID " + sectorId);
                return NotFound(new { message = "Sector no encontrado." });
            }

            return Ok(result);
        }

        /// <summary>
        /// Busca sectores basados en criterios opcionales como el ID del evento y el nombre del sector. Si se proporciona un ID de evento, se devuelven todos los sectores asociados a ese evento. Si se proporciona un nombre, se devuelven los sectores que coinciden con ese nombre (puede ser una búsqueda parcial). Si no se proporcionan criterios, se devuelven todos los sectores disponibles. La respuesta incluye una lista de sectores que cumplen con los criterios de búsqueda, o una lista vacía si no se encuentran coincidencias.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SectorResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] int? eventId, [FromQuery] string? name)
        {
            var result = await _sectorService.SearchAsync(eventId, name);
            return Ok(result);
        }
    }
}