using ReservAr.Dtos.Events;
using ReservAr.Helpers;

namespace ReservAr.Services.Interfaces
{
    public interface IPaymentServices
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request);
    }
}