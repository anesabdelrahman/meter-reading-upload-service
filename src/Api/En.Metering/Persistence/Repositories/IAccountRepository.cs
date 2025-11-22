namespace En.Metering.Persistence.Repositories
{
    public interface IAccountRepository
    {
        Task<bool> ExistsAsync(int accountId, CancellationToken token = default);
    }
}
