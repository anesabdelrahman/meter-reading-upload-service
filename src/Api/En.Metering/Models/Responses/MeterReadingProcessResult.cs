namespace En.Metering.Models.Responses
{
    public record MeterReadingProcessResult(bool IsSuccess, string? ErrorMessage = null);
}
