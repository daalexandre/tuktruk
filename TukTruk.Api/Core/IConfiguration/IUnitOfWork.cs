using TukTruk.Api.Core.IRepositories;

namespace TukTruk.Api.Core.IConfiguration
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();

        void Rollback();
    }
}