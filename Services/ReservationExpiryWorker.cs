using ReservAr.Services.Interfaces;

namespace ReservAr.Services
{
    public class ReservationExpiryWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReservationExpiryWorker> _logger;

        public ReservationExpiryWorker(IServiceProvider serviceProvider, ILogger<ReservationExpiryWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker de Expiración de Reservas iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();
                    
                    int expiredCount = await reservationService.ExpirePendingReservationsAsync();
                    if (expiredCount > 0) 
                    {
                        _logger.LogInformation($"[JOB] Se liberaron {expiredCount} reservaciones vencidas.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando expiraciones automáticas.");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}