using En.Metering.Models.Responses;
using En.Metering.Services.DTOs;

namespace En.Metering.Services.Interfaces
{
    public interface IMeterReadingProcessor
    {
        Task<MeterReadingProcessResult> ProcessRecordAsync(MeterReadingDto record, CancellationToken token);
    }
}
