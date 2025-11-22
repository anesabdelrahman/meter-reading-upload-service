
namespace En.Metering.Services.Interfaces
{
    public interface IMeterReadingUploadService
    {
        Task<(int success, int failed)> ProcessCsvAsync(Stream csvStream, CancellationToken token = default);
    }
}
