using Microsoft.EntityFrameworkCore;
using NinicoFinalTask.Models;

namespace NinicoFinalTask.DataAcces
{
    public class NinicoDbContext : DbContext
    {
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Category>  Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public NinicoDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
