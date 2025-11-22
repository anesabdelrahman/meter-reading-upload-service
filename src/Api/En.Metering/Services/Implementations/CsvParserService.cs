using CsvHelper;
using CsvHelper.Configuration;
using En.Metering.Services.DTOs;
using En.Metering.Services.Interfaces;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace En.Metering.Services.Implementations
{
    public class CsvParserService : ICsvParserService<MeterReadingDto>
    {
        public async IAsyncEnumerable<MeterReadingDto> ParseCsvAsync(Stream csvStream, [EnumeratorCancellation] CancellationToken token)
        {
            using var reader = new StreamReader(csvStream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.GetCultureInfo("en-GB"))
            {
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null
            });

            // Ensure DateTime is parsed using the expected format(s)
            var dateOptions = csv.Context.TypeConverterOptionsCache.GetOptions<DateTime>();
            dateOptions.Formats = new[] { "dd/MM/yyyy HH:mm", "dd/MM/yyyy HH:mm:ss" };

            await foreach (var record in csv.GetRecordsAsync<MeterReadingDto>(token))
            {
                yield return record;
            }
        }
    }
}
