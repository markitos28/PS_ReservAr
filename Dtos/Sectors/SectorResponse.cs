namespace ReservAr.Dtos.Sectors
{
    public class SectorResponse
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
    }
}