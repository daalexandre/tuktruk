using Microsoft.EntityFrameworkCore;
using TukTruk.Api.Models;

namespace TukTruk.Api.Data
{

    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<Truck> Trucks { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }
    }
}