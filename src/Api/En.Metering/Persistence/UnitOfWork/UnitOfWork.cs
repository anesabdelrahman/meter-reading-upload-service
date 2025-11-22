using En.Metering.Persistence.Context;

namespace En.Metering.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MeteringDbContext _context;

        public UnitOfWork(MeteringDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken token = default)
        {
            return _context.SaveChangesAsync(token);
        }
    }
}
