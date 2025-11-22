using En.Metering.Models.Domain;
using En.Metering.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace En.Metering.Persistence.Repositories
{
    public class MeterReadingRepository : IMeterReadingRepository
    {
        private readonly MeteringDbContext _context;

        public MeterReadingRepository(MeteringDbContext context)
        {
            _context = context;
        }

        //En dev: you may add the MeterReadingValue as well if you need to be very verbose about duplicate readings.
        public Task<bool> ExistsAsync(int accountId, DateTime dt, CancellationToken token = default)
        {
            return _context.MeterReadings
                .AnyAsync(x => x.AccountId == accountId && x.MeterReadingDateTime == dt, token);
        }

        public async Task AddAsync(MeterReading reading, CancellationToken token = default)
        {
            await _context.MeterReadings.AddAsync(reading, token);
        }

        public Task<MeterReading?> GetLatestAsync(int accountId, CancellationToken token = default)
        {
            return _context.MeterReadings
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.MeterReadingDateTime)
                .FirstOrDefaultAsync(token);
        }
    }
}
