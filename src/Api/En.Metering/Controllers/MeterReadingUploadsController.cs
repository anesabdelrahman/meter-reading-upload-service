using En.Metering.Services.DTOs;
using En.Metering.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace En.Metering.Controllers
{
    [ApiController]
    [Route("api/meter-readings-upload")]
    public class MeterReadingUploadsController : ControllerBase
    {
        private readonly IMeterReadingUploadService _uploadService;
        public MeterReadingUploadsController(IMeterReadingUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            using var stream = file.OpenReadStream();
            var (success, failed) = await _uploadService.ProcessCsvAsync(stream, cancellationToken);

            // En dev: Build response object. If you later return error details from the service,
            // include them here (e.g., errors variable).
            var response = new MeterReadingUploadResponse(success, failed, Errors: null);

            return Ok(response);
        }
    }
}
