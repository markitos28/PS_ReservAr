using Microsoft.AspNetCore.Mvc;
using ReservAr.Helpers;
using ReservAr.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ReservAr.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IAuditLogServices _auditLogService;
        private readonly IPaymentServices _paymentService;
        private readonly IReservationService _reservationService;

        public PaymentController(IAuditLogServices auditLogService, IPaymentServices paymentService, IReservationService reservationService)
        {
            _auditLogService = auditLogService;
            _paymentService = paymentService;
            _reservationService = reservationService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            try
            {
                var result = await _paymentService.ProcessPaymentAsync(request);
                if (!result.Success)
                {
                    await _auditLogService.Log(request.UserId, "REQUEST_PAYMENT_FAILED", "Payment", request.UserId.ToString(), $"Fallo al procesar pago por error de datos o saldo insuficiente. - EventId: {request.EventId}, SectorId: {request.SectorId}, Quantity: {request.QuantitySeat}, Amount: {request.Amount} {request.Currency} - Error: {result.Message}");
                    return BadRequest(new { message = result.Message });
                }

                // Si el pago fue exitoso, finalizamos la reserva y actualizamos los asientos de forma transaccional
                await _reservationService.ProcessPaymentAsync(request.UserId, request.ReservationId);

                await _auditLogService.Log(request.UserId, "REQUEST_PAYMENT_PROCESS", "Payment", request.UserId.ToString(), $"Pago procesado - EventId: {request.EventId}, SectorId: {request.SectorId}, Quantity: {request.QuantitySeat}, Amount: {request.Amount} {request.Currency}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                await _auditLogService.Log(-1, "REQUEST_PAYMENT_FAILED", "Payment", request.UserId.ToString(), $"Fallo al procesar pago - EventId: {request.EventId}, SectorId: {request.SectorId}, Quantity: {request.QuantitySeat}, Amount: {request.Amount} {request.Currency} - Error: {ex.Message}");
                return StatusCode(500, new { message = "Error interno al procesar el pago.", detail = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
