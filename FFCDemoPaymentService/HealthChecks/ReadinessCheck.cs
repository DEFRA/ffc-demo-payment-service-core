using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using FFCDemoPaymentService.Data;
using Microsoft.EntityFrameworkCore;

namespace FFCDemoPaymentService.HealthChecks
{

    public class ReadinessCheck : IHealthCheck
    {
        private readonly ApplicationDbContext db;

        public ReadinessCheck(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            bool databaseHealthyCheck =  await CheckDatabase(db);

            if (databaseHealthyCheck)
            {
                return
                    HealthCheckResult.Healthy("A healthy result.");
            }

            return
                HealthCheckResult.Unhealthy("An unhealthy result from Readiness check.");
        }

        private async Task<bool> CheckDatabase(ApplicationDbContext dbContext)
        {
            try
            {
                dbContext.Database.OpenConnection();
                dbContext.Database.CloseConnection();
                return await dbContext.Database.CanConnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
