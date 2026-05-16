namespace ReservAr.Helpers
{
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public int UserId { get; set; }
        public int EventId { get; set; }
        public int SectorId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}