using En.Metering.Persistence.Repositories;
using En.Metering.Services.DTOs;
using En.Metering.Validation.Interfaces;

namespace En.Metering.Validation.Implementaions
{
    public sealed class OutOfOrderMeterReadingSpecification
    : ISpecification<MeterReadingDto>
    {
        private readonly IMeterReadingRepository _repo;

        public OutOfOrderMeterReadingSpecification(IMeterReadingRepository meterReadingRepository)
        {
            _repo = meterReadingRepository;
        }

        public async Task<(bool, string)> IsSatisfiedByAsync(MeterReadingDto meterRead, CancellationToken token)
        {
            var latest = await _repo.GetLatestAsync(meterRead.AccountId, token);
            if (latest == null)
                return (true, string.Empty);

            var isNewerRead = meterRead.MeterReadingDateTime > latest.MeterReadingDateTime;

            if (!int.TryParse(meterRead.MeterReadingValue, out var newValue))
                return (false, "Invalid meter reading value.");

            if (!int.TryParse(latest.MeterReadingValue, out var latestValue))
                return (false, "Latest stored meter reading value is invalid.");

            var isLargerValue = newValue > latestValue;

            return (isNewerRead && isLargerValue)
                ? (true, string.Empty)
                : (false, "Uploaded meter reading is older or smaller than the latest one.");
        }

    }
}
