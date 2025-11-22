using En.Metering.Persistence.Repositories;
using En.Metering.Services.DTOs;
using En.Metering.Validation.Interfaces;

namespace En.Metering.Validation.Implementaions
{
    public sealed class DuplicateMeterReadingSpecification
    : ISpecification<MeterReadingDto>
    {
        private readonly IMeterReadingRepository _repo;

        public DuplicateMeterReadingSpecification(IMeterReadingRepository repo)
        {
            _repo = repo;
        }

        public async Task<(bool, string)> IsSatisfiedByAsync(MeterReadingDto candidate, CancellationToken token)
        {
            var exists = await _repo.ExistsAsync(candidate.AccountId, candidate.MeterReadingDateTime, token);
            return exists
                ? (false, "A meter read on the same date and time already exists!")
                : (true, string.Empty);
        }
    }
}
