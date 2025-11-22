namespace En.Metering.Services.Interfaces
{
    public interface ICsvParserService<T>
    {
        IAsyncEnumerable<T> ParseCsvAsync(Stream csvStream, CancellationToken token);
    }
}
