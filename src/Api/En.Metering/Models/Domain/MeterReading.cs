namespace En.Metering.Models.Domain
{
    public record MeterReading
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime MeterReadingDateTime { get; set; }
        public string MeterReadingValue { get; set; }
        public Account Account { get; set; } = default!;
    }
}
