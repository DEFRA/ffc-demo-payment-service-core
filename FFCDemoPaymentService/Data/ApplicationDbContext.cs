using Microsoft.EntityFrameworkCore;
using FFCDemoPaymentService.Models;
using System.Threading.Tasks;

namespace FFCDemoPaymentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly PostgresConnectionStringBuilder connectionStringbuilder;

        public ApplicationDbContext(PostgresConnectionStringBuilder stringBuilder) : base()
        {
            connectionStringbuilder = stringBuilder;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = Task.Run(connectionStringbuilder.GetConnectionString).Result;
            optionsBuilder.UseNpgsql(connectionString);
        }

        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
    }
}
