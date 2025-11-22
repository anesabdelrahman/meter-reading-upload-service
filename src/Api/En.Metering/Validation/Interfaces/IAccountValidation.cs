namespace En.Metering.Validation.Interfaces
{
    public interface IAccountValidation
    {
        Task<bool> AccountExistsAsync(int accountId, CancellationToken token);
    }
}
