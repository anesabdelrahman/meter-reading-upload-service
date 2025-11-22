using En.Metering.Models.Domain;

namespace En.Metering.Persistence.Repositories
{
    public interface IMeterReadingRepository
    {
        Task<bool> ExistsAsync(int accountId, DateTime readingDateTime, CancellationToken token = default);
        Task AddAsync(MeterReading reading, CancellationToken token = default);
        Task<MeterReading?> GetLatestAsync(int accountId, CancellationToken token = default);
    }
}
