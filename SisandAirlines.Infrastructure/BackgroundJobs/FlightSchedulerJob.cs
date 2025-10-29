using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SisandAirlines.Domain.Interfaces;

namespace SisandAirlines.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Hosted Service responsável por manter a agenda de voos futura atualizada.
    /// Executa diariamente à meia-noite (horário de Brasília, UTC-3),
    /// removendo voos antigos e gerando novos até 60 dias à frente.
    /// </summary>
    public class FlightSchedulerJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeZoneInfo _brTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); // Horário de Brasília

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
                    var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _brTimeZone);
                    Console.WriteLine($"🕛 [FlightSchedulerJob] Iniciando atualização de voos - {now:dd/MM/yyyy HH:mm}");

                    using var scope = _serviceProvider.CreateScope();
                    var scheduler = scope.ServiceProvider.GetRequiredService<IFlightScheduler>();

                    await scheduler.PurgeOldFlightsAsync();
                    await scheduler.GenerateFutureFlightsAsync(60);

                    var finished = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _brTimeZone);
                    Console.WriteLine($"✅ [FlightSchedulerJob] Agenda atualizada com sucesso - {finished:dd/MM/yyyy HH:mm}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ [FlightSchedulerJob] Erro durante execução: {ex.Message}");
                }

                // Calcula o tempo até a próxima meia-noite local
                var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _brTimeZone);
                var nextMidnight = nowLocal.Date.AddDays(1);
                var delay = nextMidnight - nowLocal;

                Console.WriteLine($"⏰ [FlightSchedulerJob] Próxima execução programada para: {nextMidnight:dd/MM/yyyy HH:mm}");
                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
