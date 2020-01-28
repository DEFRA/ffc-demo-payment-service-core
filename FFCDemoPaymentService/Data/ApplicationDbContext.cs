using Microsoft.EntityFrameworkCore;
using FFCDemoPaymentService.Models;

namespace FFCDemoPaymentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() {}
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Schedule> Schedule { get; set; } 
        public virtual DbSet<Payment> Payments { get; set; } 
    }
}
