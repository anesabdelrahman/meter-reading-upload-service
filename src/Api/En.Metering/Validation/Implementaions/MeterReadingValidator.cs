using En.Metering.Services.DTOs;
using FluentValidation;

namespace En.Metering.Validation.Implementaions
{
    public class MeterReadingValueValidator : AbstractValidator<MeterReadingDto>
    {
        public MeterReadingValueValidator()
        {
            RuleFor(x => x.AccountId).GreaterThan(0);
            RuleFor(x => x.MeterReadingDateTime).NotEmpty();
            RuleFor(x => x.MeterReadingValue)
                .Must(BeAValidMeterReading)
                .WithMessage("Meter reading must be a 5-digit number");
        }

        private bool BeAValidMeterReading(string value)
        {
            return int.TryParse(value, out int reading) &&
                   reading >= 0 &&
                   reading <= 99999;
        }
    }
}
