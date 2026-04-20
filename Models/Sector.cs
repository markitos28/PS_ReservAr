namespace ReservAr.Models
{
    public class Sector
    {
        public int Id { get; set; } //Identificador único del sector
        public int EventId { get; set; } //Identificador del evento al que pertenece el sector
        public string Name { get; set; } //Nombre del sector
        public decimal Price { get; set; } //Precio por asiento
        public int Capacity { get; set; } //Capacidad total del sector
    }
}