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

        public void Log(int userId, string action, string entityType,  string entityId,  string details)
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
            _dbContext.SaveChanges();
        }
        
        
    }
}