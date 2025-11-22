using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using En.Metering.Controllers;
using En.Metering.Services.Interfaces;
using En.Metering.Services.DTOs;

namespace MeteringTests.Controllers
{
    [TestFixture]
    public class ControllerTests
    {
        [Test]
        public async Task Upload_ReturnsBadRequest_WhenFileIsNull()
        {
            var serviceMock = new Mock<IMeterReadingUploadService>();
            var controller = new MeterReadingUploadsController(serviceMock.Object);

            var result = await controller.Upload(null, CancellationToken.None);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Upload_ReturnsOk_WithValidFile()
        {
            var csv = "AccountId,MeterReadingDateTime,MeterReadingValue\n1001,22/04/2019 12:25,12345";
            var bytes = Encoding.UTF8.GetBytes(csv);
            using var stream = new MemoryStream(bytes);

            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };

            var serviceMock = new Mock<IMeterReadingUploadService>();
            serviceMock
                .Setup(x => x.ProcessCsvAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((1, 0)); // service still returns a tuple internally

            var controller = new MeterReadingUploadsController(serviceMock.Object);

            var result = await controller.Upload(file, CancellationToken.None);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var ok = (OkObjectResult)result;

            // Controller now wraps the tuple into MeterReadingUploadResponse; assert against the DTO
            Assert.IsInstanceOf<MeterReadingUploadResponse>(ok.Value);
            var response = ok.Value as MeterReadingUploadResponse;
            Assert.Multiple(() =>
            {
                Assert.That(response?.Success, Is.EqualTo(1));
                Assert.That(response?.Failed, Is.EqualTo(0));
            });
        }

        [Test]
        public async Task QueryController_GetAll_ReturnsOk()
        {
            var serviceMock = new Mock<IMeterReadingQueryService>();
            serviceMock.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<En.Metering.Models.Domain.MeterReading>());

            var controller = new MeterReadingsQueryController(serviceMock.Object);

            var result = await controller.GetAll(CancellationToken.None);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task QueryController_GetByAccount_ReturnsOk()
        {
            var serviceMock = new Mock<IMeterReadingQueryService>();
            serviceMock.Setup(s => s.GetByAccountAsync(1001, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<En.Metering.Models.Domain.MeterReading>());

            var controller = new MeterReadingsQueryController(serviceMock.Object);

            var result = await controller.GetByAccount(1001);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task QueryController_Summaries_ReturnOk()
        {
            var serviceMock = new Mock<IMeterReadingQueryService>();
            serviceMock.Setup(s => s.GetDailySummaryAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DailyMeterReadSummaryDto>());
            serviceMock.Setup(s => s.GetTopAccountsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TopAccountUsageDto>());

            var controller = new MeterReadingsQueryController(serviceMock.Object);

            Assert.IsInstanceOf<OkObjectResult>(await controller.GetDailySummary(CancellationToken.None));
            Assert.IsInstanceOf<OkObjectResult>(await controller.GetTopAccounts(CancellationToken.None));
        }
    }
}
