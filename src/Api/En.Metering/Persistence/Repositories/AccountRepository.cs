using En.Metering.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace En.Metering.Persistence.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly MeteringDbContext _context;

        public AccountRepository(MeteringDbContext context)
        {
            _context = context;
        }

        public Task<bool> ExistsAsync(int accountId, CancellationToken token = default)
        {
            return _context.Accounts
                .AnyAsync(x => x.AccountId == accountId, token);
        }
    }
}
