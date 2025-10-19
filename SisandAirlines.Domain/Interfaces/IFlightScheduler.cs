namespace SisandAirlines.Domain.Interfaces
{
    public interface IFlightScheduler
    {
        Task PurgeOldFlightsAsync();
        Task GenerateFutureFlightsAsync(int daysAhead = 60);
    }
}
