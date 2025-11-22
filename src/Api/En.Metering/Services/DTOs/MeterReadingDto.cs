namespace En.Metering.Services.DTOs
{
    public record MeterReadingDto
    {
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public required string MeterReadingValue { get; set; }
    }
}
