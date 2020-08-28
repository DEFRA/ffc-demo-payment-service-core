using Microsoft.EntityFrameworkCore;
using FFCDemoPaymentService.Models;
using System.Threading.Tasks;

namespace FFCDemoPaymentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly SchemaConfig schemaConfig;
        private readonly PostgresConnectionStringBuilder connectionStringBuilder;

        public ApplicationDbContext() { }
        public ApplicationDbContext(PostgresConnectionStringBuilder stringBuilder) : base()
        {
            connectionStringBuilder = stringBuilder;
            schemaConfig = null;
        }

        public ApplicationDbContext(PostgresConnectionStringBuilder stringBuilder, SchemaConfig schemaConfig) : base()
        {
            connectionStringBuilder = stringBuilder;
            this.schemaConfig = schemaConfig;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = Task.Run(connectionStringBuilder.GetConnectionString).Result;
            optionsBuilder.UseNpgsql(connectionString);
        }

        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (schemaConfig != null)
            {
                modelBuilder.HasDefaultSchema(schemaConfig.Default);
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
