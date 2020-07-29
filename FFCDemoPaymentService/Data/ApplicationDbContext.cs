using Microsoft.EntityFrameworkCore;
using FFCDemoPaymentService.Models;
using System.Threading.Tasks;

namespace FFCDemoPaymentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ConnectionStringBuilder builder;

        public ApplicationDbContext(ConnectionStringBuilder builder)
            : base()
        {
            this.builder = builder;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = Task.Run(builder.GetConnectionString).Result;
            System.Console.WriteLine("Using Connection String:");
            System.Console.WriteLine($"{connectionString}");
            optionsBuilder.UseNpgsql(connectionString);
        }

        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
    }
}
