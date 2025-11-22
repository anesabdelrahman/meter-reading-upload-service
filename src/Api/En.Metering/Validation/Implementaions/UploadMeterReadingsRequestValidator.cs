using En.Metering.Models.Requests;
using FluentValidation;

namespace En.Metering.Validation.Implementaions
{
    public class UploadMeterReadingsRequestValidator : AbstractValidator<UploadMeterReadingsRequest>
    {
        public UploadMeterReadingsRequestValidator()
        {
            RuleFor(x => x.File)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("File is required.")
                .Must(f => f.Length > 0).WithMessage("File cannot be empty.")
                .Must(f => f.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only CSV files are supported.");
        }
    }
}
