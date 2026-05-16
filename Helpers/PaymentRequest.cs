namespace ReservAr.Helpers
{
    public class PaymentRequest
    {
        public int EventId { get; set; }
        public int SectorId { get; set; }
        public int QuantitySeat { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        
    }
}