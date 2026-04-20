namespace ReservAr.Models
{
    public class Event
    {
        public int Id { get; set; } //Identificador único del evento
        public string Name { get; set; } //Nombre del evento
        public DateTime EventDate { get; set; } //Fecha del evento
        public string Venue { get; set; } //Lugar del evento
        public string Status { get; set; } //Activo, Cancelado, etc.
    }
}