using En.Metering.Models.Domain;
using En.Metering.Services.DTOs;

namespace En.Metering.Services.Interfaces
{
    public interface IMeterReadingQueryService
    {
        Task<List<MeterReading>> GetAllAsync(CancellationToken token = default);
        Task<List<MeterReading>> GetByAccountAsync(int accountId, CancellationToken token = default);
        Task<List<TopAccountUsageDto>> GetTopAccountsAsync(CancellationToken token = default);
        Task<List<DailyMeterReadSummaryDto>> GetDailySummaryAsync(CancellationToken token = default);
    }
}
