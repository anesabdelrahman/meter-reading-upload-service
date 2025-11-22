using System.Text;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using En.Metering.Models.Requests;
using En.Metering.Validation;
using En.Metering.Services.DTOs;
using En.Metering.Validation.Implementaions;

namespace MeteringTests.Validation
{
    [TestFixture]
    public class ValidationTests
    {
        [TestCase("12345", true)]
        [TestCase("1234", true)]
        [TestCase("123456", false)]
        [TestCase("-1234", false)]
        [TestCase("abcde", false)]
        public void MeterReadingValueValidator_Validates(string input, bool expectedValid)
        {
            var validator = new MeterReadingValueValidator();

            var dto = new MeterReadingDto
            {
                AccountId = 1,
                MeterReadingDateTime = DateTime.Now,
                MeterReadingValue = input
            };

            var result = validator.Validate(dto);
            Assert.That(result.IsValid, Is.EqualTo(expectedValid));
        }

        [Test]
        public void UploadRequestValidator_Rejects_NullFile()
        {
            var validator = new UploadMeterReadingsRequestValidator();
            var req = new UploadMeterReadingsRequest { File = null! };
            var result = validator.Validate(req);
            Assert.That(result.IsValid, Is.False);
            Assert.That(result.Errors, Has.Some.Matches<ValidationFailure>(e => e.PropertyName == "File"));
        }

        [Test]
        public void UploadRequestValidator_Rejects_EmptyFile()
        {
            var emptyStream = new MemoryStream();
            var file = new FormFile(emptyStream, 0, 0, "file", "empty.csv")
            {
                Headers = new HeaderDictionary()
            };

            var validator = new UploadMeterReadingsRequestValidator();
            var req = new UploadMeterReadingsRequest { File = file };
            var result = validator.Validate(req);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void UploadRequestValidator_Rejects_NonCsv()
        {
            var bytes = Encoding.UTF8.GetBytes("data");
            using var stream = new MemoryStream(bytes);
            var file = new FormFile(stream, 0, stream.Length, "file", "test.txt")
            {
                Headers = new HeaderDictionary()
            };

            var validator = new UploadMeterReadingsRequestValidator();
            var req = new UploadMeterReadingsRequest { File = file };
            var result = validator.Validate(req);
            Assert.That(result.IsValid, Is.False);
        }

        [Test]
        public void UploadRequestValidator_Accepts_ValidCsv()
        {
            var bytes = Encoding.UTF8.GetBytes("a");
            using var stream = new MemoryStream(bytes);
            var file = new FormFile(stream, 0, stream.Length, "file", "test.csv")
            {
                Headers = new HeaderDictionary()
            };

            var validator = new UploadMeterReadingsRequestValidator();
            var req = new UploadMeterReadingsRequest { File = file };
            var result = validator.Validate(req);
            Assert.That(result.IsValid, Is.True);
        }
    }
}
