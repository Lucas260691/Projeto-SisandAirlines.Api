using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SisandAirlines.Domain.Interfaces;

namespace SisandAirlines.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Hosted Service responsável por manter a agenda de voos futura atualizada.
    /// Roda diariamente às 00:00 (UTC-3), removendo voos antigos e gerando novos até 60 dias à frente.
    /// </summary>
    public class FlightSchedulerJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24); // executa 1x a cada 24h

        public FlightSchedulerJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Aguarda alguns segundos antes da primeira execução (útil para ambientes Docker)
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var scheduler = scope.ServiceProvider.GetRequiredService<IFlightScheduler>();

                    Console.WriteLine($"🕛 [FlightSchedulerJob] Iniciando atualização de voos - {DateTime.Now:dd/MM/yyyy HH:mm}");

                    await scheduler.PurgeOldFlightsAsync();
                    await scheduler.GenerateFutureFlightsAsync(60);
                    
                    Console.WriteLine($"✅ [FlightSchedulerJob] Agenda atualizada com sucesso - {DateTime.Now:dd/MM/yyyy HH:mm}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ [FlightSchedulerJob] Erro durante execução: {ex.Message}");
                }

                // Aguarda até o próximo ciclo (24h)
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
