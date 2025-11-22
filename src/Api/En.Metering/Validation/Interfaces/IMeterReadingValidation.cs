using En.Metering.Services.DTOs;

namespace En.Metering.Validation.Interfaces
{
    public interface IMeterReadingValidation
    {
        Task<(bool, string)> MeterReadingIsValidAsync(MeterReadingDto meterRead, CancellationToken token);
    }
}
