using En.Metering.Persistence.Repositories;
using En.Metering.Validation.Interfaces;

namespace En.Metering.Validation.Implementaions
{
    public class AccountValidation : IAccountValidation
    {
        private readonly IAccountRepository _accountRepository;

        public AccountValidation(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Task<bool> AccountExistsAsync(int accountId, CancellationToken token)
        {
            return _accountRepository.ExistsAsync(accountId, token);
        }
    }
}
