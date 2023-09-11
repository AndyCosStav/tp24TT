using Microsoft.EntityFrameworkCore;
using tp24TT.Data.DataModels;

namespace tp24TT.Data
{
    public class Tp24ttContext : DbContext
    {

        public Tp24ttContext(DbContextOptions<Tp24ttContext> options)
             : base(options)
        {

        }

        public DbSet<Receivable> Receivables { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Receivable>()
                .HasKey(e => e.Reference); 
        }

    }
}
