using En.Metering.Models.Domain;
using En.Metering.Persistence.Context;
using En.Metering.Services.DTOs;
using En.Metering.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace En.Metering.Services.Implementations
{
    public class MeterReadingQueryService : IMeterReadingQueryService
    {
        private readonly MeteringDbContext _context;

        public MeterReadingQueryService(MeteringDbContext context) => _context = context;

        public async Task<List<MeterReading>> GetAllAsync(CancellationToken token = default) =>
            await _context.MeterReadings
                          .AsNoTracking()
                          .OrderBy(x => x.MeterReadingDateTime)
                          .ToListAsync(token);

        public async Task<List<MeterReading>> GetByAccountAsync(int accountId, CancellationToken token = default) =>
            await _context.MeterReadings
                          .AsNoTracking()
                          .Where(x => x.AccountId == accountId)
                          .OrderByDescending(x => x.MeterReadingDateTime)
                          .ToListAsync(token);

        public async Task<List<DailyMeterReadSummaryDto>> GetDailySummaryAsync(CancellationToken token = default)
        {
            return await _context.MeterReadings
                .GroupBy(x => x.MeterReadingDateTime.Date)
                .Select(g => new DailyMeterReadSummaryDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Date)
                .ToListAsync(token);
        }

        public async Task<List<TopAccountUsageDto>> GetTopAccountsAsync(CancellationToken token = default)
        {
            return await _context.MeterReadings
                .GroupBy(x => x.AccountId)
                .Select(g => new TopAccountUsageDto
                {
                    AccountId = g.Key,
                    SubmissionCount = g.Count()
                })
                .OrderByDescending(x => x.SubmissionCount)
                .Take(10)
                .ToListAsync(token);
        }
    }
}
