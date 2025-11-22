namespace En.Metering.Models.Domain
{
    public record Account
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public ICollection<MeterReading> MeterReadings { get; set; } = [];
    }
}
