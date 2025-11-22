
namespace En.Metering.Services.DTOs
{
    public record MeterReadingUploadResponse(int Success, int Failed, IReadOnlyList<string>? Errors = null);
}