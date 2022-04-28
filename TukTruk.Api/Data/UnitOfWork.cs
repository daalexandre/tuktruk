using TukTruk.Api.Core.IConfiguration;
using TukTruk.Api.Core.IRepositories;
using TukTruk.Api.Core.Repositories;
using TukTruk.Api.Data;

namespace TukTruk.Data
{
    public class UnitOfWork : IUnitOfWork, System.IDisposable
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(
            ApplicationDbContext context
        )
        {
            _context = context;
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Rollback()
        {

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}