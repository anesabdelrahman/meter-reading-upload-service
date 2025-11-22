namespace En.Metering.Services.DTOs
{
    public record TopAccountUsageDto
    {
        public int AccountId { get; set; }
        public int SubmissionCount { get; set; }
    }
}
