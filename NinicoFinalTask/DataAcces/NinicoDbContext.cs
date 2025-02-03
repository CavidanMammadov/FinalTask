using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.Models;

namespace NinicoFinalTask.DataAcces
{
    public class NinicoDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public NinicoDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
