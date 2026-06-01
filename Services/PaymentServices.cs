using ReservAr.Services.Interfaces;
using ReservAr.Dtos.Events;
using ReservAr.Helpers;


namespace ReservAr.Services
{
    public class PaymentServices : IPaymentServices
    {
        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request)
        {
            await Task.Delay(5000); // Simula un retraso en el procesamiento
            PaymentResponse paymentResult = new PaymentResponse
            {
                Success = true,
                TransactionId = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                Amount = request.Amount,
                EventId = request.EventId,
                SectorId = request.SectorId,
                Currency = request.Currency,
                Message = "Pago procesado exitosamente."
            };

            // Deberías reemplazar esto con una respuesta real basada en tu modelo de datos.
            return paymentResult;
        }
    }
}