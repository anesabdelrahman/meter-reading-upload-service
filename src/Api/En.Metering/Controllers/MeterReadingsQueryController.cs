using En.Metering.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace En.Metering.Controllers
{
    [ApiController]
    [Route("api/meter-readings-queries")]
    public class MeterReadingsQueryController : ControllerBase
    {
        private readonly IMeterReadingQueryService _queryService;

        public MeterReadingsQueryController(IMeterReadingQueryService service) => _queryService = service;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToke) =>
            Ok(await _queryService.GetAllAsync(cancellationToke));

        [HttpGet("{accountId:int}")]
        public async Task<IActionResult> GetByAccount(int accountId) =>
            Ok(await _queryService.GetByAccountAsync(accountId));

        [HttpGet("summary/daily")]
        public async Task<IActionResult> GetDailySummary(CancellationToken cancellationToke) =>
            Ok(await _queryService.GetDailySummaryAsync(cancellationToke));

        [HttpGet("summary/top")]
        public async Task<IActionResult> GetTopAccounts(CancellationToken cancellationToke) =>
            Ok(await _queryService.GetTopAccountsAsync(cancellationToke));
    }
}
