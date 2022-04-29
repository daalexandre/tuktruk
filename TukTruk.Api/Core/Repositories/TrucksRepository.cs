using Microsoft.EntityFrameworkCore;
using TukTruk.Api.Data;
using TukTruk.Api.Models;
using TukTruk.Api.Core.IRepositories;

namespace TukTruk.Api.Core.Repositories
{
    public class TrucksRepository : GenericRepository<Truck>, ITrucksRepository
    {
        public TrucksRepository(
            ApplicationDbContext context
        ) : base(context)
        {

        }

        public override async Task<IEnumerable<Truck>> All()
        {
            return await dbSet.ToListAsync();
        }

        public override async Task<bool> Update(Truck entity)
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

        public override async Task<bool> Delete(Guid id)
        {
            var existingTruck = await dbSet.Where(t => t.Id == id).FirstOrDefaultAsync();
            if (existingTruck != null)
            {
                dbSet.Remove(existingTruck);
                return true;
            }
            return false;
        }

    }
}