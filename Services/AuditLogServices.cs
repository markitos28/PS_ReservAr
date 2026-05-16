using ReservAr.Data;
using ReservAr.Models;
using ReservAr.Services.Interfaces;

namespace ReservAr.Services
{
    public class AuditLogServices : IAuditLogServices
    {
        private readonly ReservArDbContext _dbContext;

        public AuditLogServices(ReservArDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Log(int userId, string action, string entityType,  string entityId,  string details)
        {   
            try
            {
                var auditLog = new Audit_Log
                {
                    Id= Guid.NewGuid(),
                    UserId = userId,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    Details = details,
                    CreatedAt = DateTime.UtcNow
                };

                _dbContext.AuditLogs.Add(auditLog);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Aquí podrías loguear el error a un sistema de logging o simplemente ignorarlo.
                Console.WriteLine($"Error al guardar el log de auditoría: {ex.Message}");
            }
        }
    }
}