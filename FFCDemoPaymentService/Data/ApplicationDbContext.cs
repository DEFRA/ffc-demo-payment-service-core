using Microsoft.EntityFrameworkCore;
using FFCDemoPaymentService.Models;

namespace FFCDemoPaymentService.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly SchemaConfig schemaConfig;

        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, SchemaConfig schemaConfig)
            : base(options)
        {
            this.schemaConfig = schemaConfig;
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
