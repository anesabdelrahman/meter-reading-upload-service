using En.Metering.Persistence.Repositories;
using En.Metering.Services.DTOs;
using En.Metering.Validation.Interfaces;

namespace En.Metering.Validation.Implementaions
{
    public class MeterReadingValidationService : IMeterReadingValidation
    {
        private readonly ISpecification<MeterReadingDto> _specification;

        public MeterReadingValidationService(IMeterReadingRepository meterReadingRepository)
        {
            var duplicateSpec = new DuplicateMeterReadingSpecification(meterReadingRepository);
            var outOfOrderSpec = new OutOfOrderMeterReadingSpecification(meterReadingRepository);

            _specification = new AndSpecification<MeterReadingDto>(duplicateSpec, outOfOrderSpec);
        }

        public async Task<(bool, string)> MeterReadingIsValidAsync(MeterReadingDto meterRead, CancellationToken token)
        {
            return await _specification.IsSatisfiedByAsync(meterRead, token);
        }
    }
}
