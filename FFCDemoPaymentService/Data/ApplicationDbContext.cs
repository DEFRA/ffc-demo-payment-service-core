using Microsoft.EntityFrameworkCore;
using FFCDemoPaymentService.Models;
using System.Threading.Tasks;
using System;

namespace FFCDemoPaymentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly SchemaConfig schemaConfig;
        private readonly PostgresConnectionStringBuilder connectionStringbuilder;

        public ApplicationDbContext() { }
        public ApplicationDbContext(PostgresConnectionStringBuilder stringBuilder, SchemaConfig schemaConfig) : base()
        {
            connectionStringbuilder = stringBuilder;
            this.schemaConfig = schemaConfig;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = Task.Run(connectionStringbuilder.GetConnectionString).Result;
            optionsBuilder.UseNpgsql(connectionString);
        }

        public virtual DbSet<Schedule> Schedule { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Console.WriteLine("***************ONMODELCREATING");
            if (schemaConfig != null)
            {
                modelBuilder.HasDefaultSchema(schemaConfig.Default);
            }
            base.OnModelCreating(modelBuilder);
        }
    }
}
