

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace EF_DotNetCore.Models
{
    public class EmployessDBContext : IdentityDbContext<ApplicaitonUsers>
    {
        public EmployessDBContext(DbContextOptions<EmployessDBContext> obj) :base (obj)
        {

        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LogIn> LogIn { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 
            modelBuilder.Entity<LogIn>()
                .HasNoKey();

            
        }
    }
}
