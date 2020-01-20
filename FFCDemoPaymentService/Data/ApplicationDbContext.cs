using Microsoft.EntityFrameworkCore;
using FFCDemoPaymentService.Models;

namespace FFCDemoPaymentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Schedule> Schedule { get; set; } 
        public DbSet<Payment> Payments { get; set; } 
    }
}
