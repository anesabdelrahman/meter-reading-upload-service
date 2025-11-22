namespace En.Metering.Models.Requests
{
    public class UploadMeterReadingsRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
