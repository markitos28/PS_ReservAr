namespace ReservAr.Models
{
    public class Audit_Log
    {
        public Guid Id {get; set;} //Identificador único del log
        public int? UserId { get; set; } //Identificador del usuario que realizó la acción
        public string Action { get; set; } //Descripción de la acción realizada (e.g., "Reservar_Asiento", "Cancelar_Evento")
        public string EntityType { get; set; } //Tipo de entidad afectada (Ej: Reservacion, Evento, Sector)
        public string EntityId { get; set; } //Identificador de la entidad afectada
        public required string Details { get; set; } //JSON con metadatos del evento 
        public DateTime CreatedAt { get; set; } //Fecha y hora en que se creó el registro
    }
}