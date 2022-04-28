using Microsoft.EntityFrameworkCore;
using TukTruk.Api.Data;
using TukTruk.Api.Models;
using TukTruk.Api.Core.IRepositories;

namespace TukTruk.Api.Core.Repositories
{
    public class TrucksRepository : GenericRepository<Truck>, ITrucksRepository
    {
        public TrucksRepository(
            ApplicationDbContext context,
            ILogger logger
        ) : base(context, logger)
        {

        }

        public override async Task<IEnumerable<Truck>> All()
        {
            try
            {
                return await dbSet.ToListAsync();
            }
            catch (System.Exception ex)
            {

                _logger.LogError(ex, $"[{nameof(TrucksRepository)}] -> {nameof(All)}");
                return new List<Truck>();
            }
        }

        public override async Task<bool> Update(Truck entity)
        {
            try
            {
                var existingTruck = await dbSet.Where(t => t.Id == entity.Id).FirstOrDefaultAsync();
                if (existingTruck != null)
                {
                    existingTruck.ManufacturingYear = entity.ManufacturingYear;
                    existingTruck.ModelYear = entity.ModelYear;
                    existingTruck.Model = entity.Model;

                    dbSet.Update(existingTruck);

                }

                return true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(TrucksRepository)}] -> {nameof(Update)}");
                return false;
            }
        }

        public override async Task<bool> Delete(Guid id)
        {
            try
            {
                var existingTruck = await dbSet.Where(t => t.Id == id).FirstOrDefaultAsync();
                if (existingTruck != null)
                {
                    dbSet.Remove(existingTruck);
                    return true;
                }
                return false;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, $"[{nameof(TrucksRepository)}] -> {nameof(Delete)}");
                return false;
            }
        }

    }
}