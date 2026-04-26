namespace ReservAr.Services.Interfaces
{
    /// <summary>
    /// Interface para el servicio de registro de auditoría. Permite registrar eventos importantes o acciones realizadas en el sistema para fines de seguimiento y análisis.
    /// </summary>
    public interface IAuditLogService
    {
        void Log(int userId, string action, string entityType, string entityId, string details);
    }
}