using En.Metering.Models.Domain;
using En.Metering.Models.Responses;
using En.Metering.Persistence.Repositories;
using En.Metering.Persistence.UnitOfWork;
using En.Metering.Services.DTOs;
using En.Metering.Services.Interfaces;
using En.Metering.Validation.Interfaces;
using FluentValidation;

namespace En.Metering.Services.Implementations
{
    public class MeterReadingProcessor : IMeterReadingProcessor
    {
        private readonly IMeterReadingRepository _meterRepo;
        private readonly IValidator<MeterReadingDto> _readingValidator;
        private readonly IAccountValidation _accountValidator;
        private readonly IMeterReadingValidation _meterReadingValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Serilog.ILogger _logger;

        public MeterReadingProcessor(
            IMeterReadingRepository meterRepo,
            IValidator<MeterReadingDto> readingValidator,
            IAccountValidation accountValidator,
            IMeterReadingValidation meterReadingValidationService,
            IUnitOfWork unitOfWork,
            Serilog.ILogger logger)
        {
            _meterRepo = meterRepo;
            _readingValidator = readingValidator;
            _accountValidator = accountValidator;
            _meterReadingValidator = meterReadingValidationService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<MeterReadingProcessResult> ProcessRecordAsync(MeterReadingDto record, CancellationToken token)
        {
            try
            {
                var validationResult = await _readingValidator.ValidateAsync(record, token);
                if (!validationResult.IsValid)
                {
                    return new MeterReadingProcessResult(false,
                        $"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
                }

                var accountId = record.AccountId;
                var meterReadingDateTime = TruncateToMinute(record.MeterReadingDateTime);
                var meterReadingValue = record.MeterReadingValue;

                if (!await _accountValidator.AccountExistsAsync(accountId, token))
                {
                    return new MeterReadingProcessResult(false, $"Account {accountId} not found");
                }

                var (isValid, specMessage) = await _meterReadingValidator.MeterReadingIsValidAsync(record, token);
                if (!isValid)
                {
                    var message = string.IsNullOrWhiteSpace(specMessage)
                        ? $"Meter reading for account {accountId} on {meterReadingDateTime} is invalid"
                        : specMessage;
                    return new MeterReadingProcessResult(false, message);
                }

                var reading = new MeterReading
                {
                    AccountId = accountId,
                    MeterReadingDateTime = meterReadingDateTime,
                    MeterReadingValue = meterReadingValue
                };

                await _meterRepo.AddAsync(reading, token);
                await _unitOfWork.SaveChangesAsync(token);

                return new MeterReadingProcessResult(true);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing meter reading for account {AccountId}", record.AccountId);
                return new MeterReadingProcessResult(false, $"Processing error: {ex.Message}");
            }
        }

        private static DateTime TruncateToMinute(DateTime dateTime)
        {
            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                dateTime.Hour,
                dateTime.Minute,
                0,
                dateTime.Kind);
        }
    }
}