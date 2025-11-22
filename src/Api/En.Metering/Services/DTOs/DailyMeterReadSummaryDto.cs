namespace En.Metering.Services.DTOs
{
    public record DailyMeterReadSummaryDto
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
