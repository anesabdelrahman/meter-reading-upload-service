using System.Text;
using En.Metering.Models.Domain;
using En.Metering.Models.Responses;
using En.Metering.Persistence.Repositories;
using En.Metering.Persistence.UnitOfWork;
using En.Metering.Services.DTOs;
using En.Metering.Services.Implementations;
using En.Metering.Services.Interfaces;
using En.Metering.Validation.Interfaces;
using FluentValidation;
using Moq;
using Serilog;

namespace MeteringTests.Services
{
    [TestFixture]
    public class MeterReadingUploadServiceTests
    {
        private Mock<IMeterReadingProcessor> _processorMock;
        private Mock<ICsvParserService<MeterReadingDto>> _csvParserMock;
        private Mock<ILogger> _loggerMock;
        private MeterReadingUploadService _service;

        [SetUp]
        public void Setup()
        {
            _processorMock = new Mock<IMeterReadingProcessor>();
            _csvParserMock = new Mock<ICsvParserService<MeterReadingDto>>();
            _loggerMock = new Mock<ILogger>();
            _service = new MeterReadingUploadService(
                _processorMock.Object,
                _csvParserMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task ProcessCsvAsync_WithMixedRecords_ReturnsCorrectSuccessAndFailedCounts()
        {
            // Arrange
            var records = new List<MeterReadingDto>
            {
                new MeterReadingDto { AccountId = 1001, MeterReadingDateTime = new DateTime(2024,11,14,15,30,12), MeterReadingValue = "12345" },
                new MeterReadingDto { AccountId = 9999, MeterReadingDateTime = new DateTime(2024,11,15,15,30,12), MeterReadingValue = "1285" }
            };

            _csvParserMock
                .Setup(x => x.ParseCsvAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(ToAsyncEnumerable(records));

            _processorMock
                .SetupSequence(x => x.ProcessRecordAsync(It.IsAny<MeterReadingDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MeterReadingProcessResult(true))
                .ReturnsAsync(new MeterReadingProcessResult(false, "Account not found"));

            var csvContent = new StringBuilder();
            csvContent.AppendLine("AccountId,MeterReadingDateTime,MeterReadingValue");
            csvContent.AppendLine("1001,14/11/2024 15:30,12345");
            csvContent.AppendLine("9999,15/11/2024 15:30,1285");

            var bytes = Encoding.UTF8.GetBytes(csvContent.ToString());
            using var stream = new MemoryStream(bytes);

            // Act
            var (success, failed) = await _service.ProcessCsvAsync(stream, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.EqualTo(1));
                Assert.That(failed, Is.EqualTo(1));
            });

            _csvParserMock.Verify(x => x.ParseCsvAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
            _processorMock.Verify(x => x.ProcessRecordAsync(It.IsAny<MeterReadingDto>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Test]
        public async Task ProcessCsvAsync_WhenCancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            var records = new List<MeterReadingDto>
            {
                new MeterReadingDto { AccountId = 1001, MeterReadingDateTime = new DateTime(2019,04,22,12,25,12), MeterReadingValue = "12345" }
            };

            _csvParserMock
                .Setup(x => x.ParseCsvAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(ToAsyncEnumerable(records));

            var cts = new CancellationTokenSource();
            cts.Cancel();

            var csvContent = "AccountId,MeterReadingDateTime,MeterReadingValue\n1001,22/04/2019 12:25,12345";
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            using var stream = new MemoryStream(bytes);

            // Act & Assert
            Assert.ThrowsAsync<OperationCanceledException>(() =>
                _service.ProcessCsvAsync(stream, cts.Token));
        }

        [Test]
        public async Task ProcessCsvAsync_WithEmptyFile_ReturnsZeroSuccessAndFailed()
        {
            // Arrange
            _csvParserMock
                .Setup(x => x.ParseCsvAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(AsyncEnumerable.Empty<MeterReadingDto>());

            var csvContent = "AccountId,MeterReadingDateTime,MeterReadingValue";
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            using var stream = new MemoryStream(bytes);

            // Act
            var (success, failed) = await _service.ProcessCsvAsync(stream, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.EqualTo(0));
                Assert.That(failed, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task ProcessCsvAsync_WhenProcessorThrows_LogsErrorAndIncrementsFailed()
        {
            // Arrange
            var records = new List<MeterReadingDto>
            {
                new MeterReadingDto { AccountId = 1001, MeterReadingDateTime = new DateTime(2019,04,22,12,25,12), MeterReadingValue = "12345" }
            };

            _csvParserMock
                .Setup(x => x.ParseCsvAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns(ToAsyncEnumerable(records));

            _processorMock
                .Setup(x => x.ProcessRecordAsync(It.IsAny<MeterReadingDto>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));

            var csvContent = "AccountId,MeterReadingDateTime,MeterReadingValue\n1001,22/04/2019 12:25,12345";
            var bytes = Encoding.UTF8.GetBytes(csvContent);
            using var stream = new MemoryStream(bytes);

            // Act
            var (success, failed) = await _service.ProcessCsvAsync(stream, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(success, Is.EqualTo(0));
                Assert.That(failed, Is.EqualTo(1));
            });

            _loggerMock.Verify(
                x => x.Warning(It.IsAny<string>(), It.IsAny<string>()),
                Times.Once);
        }

        private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                yield return item;
                await Task.Yield();
            }
        }
    }

    [TestFixture]
    public class MeterReadingProcessorTests
    {
        private Mock<IMeterReadingRepository> _repositoryMock;
        private Mock<IValidator<MeterReadingDto>> _validatorMock;
        private Mock<IAccountValidation> _accountValidatorMock;
        Mock<IMeterReadingValidation> _meterReadingValidatorMock;
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ILogger> _loggerMock;
        private MeterReadingProcessor _processor;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IMeterReadingRepository>();
            _validatorMock = new Mock<IValidator<MeterReadingDto>>();
            _accountValidatorMock = new Mock<IAccountValidation>();
            _meterReadingValidatorMock = new Mock<IMeterReadingValidation>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger>();

            _processor = new MeterReadingProcessor(
                _repositoryMock.Object,
                _validatorMock.Object,
                _accountValidatorMock.Object,
                _meterReadingValidatorMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task ProcessRecordAsync_WithValidRecord_ReturnsSuccess()
        { 
            // Arrange
            var record = new MeterReadingDto {AccountId = 1001, MeterReadingDateTime = DateTime.Parse("12/04/2019 12:25"), MeterReadingValue = "12345" };

            _accountValidatorMock
                .Setup(x => x.AccountExistsAsync(1001, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _meterReadingValidatorMock
                .Setup(x => x.MeterReadingIsValidAsync(It.IsAny<MeterReadingDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((true, string.Empty));

            _validatorMock
                .Setup(x => x.ValidateAsync(record, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            _repositoryMock
                .Setup(x => x.ExistsAsync(1001, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _processor.ProcessRecordAsync(record, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.ErrorMessage, Is.Null);
            });

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MeterReading>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task ProcessRecordAsync_WithInvalidAccount_ReturnsFailure()
        {
            // Arrange
            var record = new MeterReadingDto { AccountId = 1001, MeterReadingDateTime = new DateTime(2019, 04, 22, 12, 25, 12), MeterReadingValue = "12345" };

            _accountValidatorMock
                .Setup(x => x.AccountExistsAsync(9999, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            _validatorMock
                .Setup(x => x.ValidateAsync(record, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());

            // Act
            var result = await _processor.ProcessRecordAsync(record, CancellationToken.None);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.ErrorMessage, Is.EqualTo("Account 1001 not found"));
            });

            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<MeterReading>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}