using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using FFCDemoPaymentService.Data;

public class ReadinessCheck : IHealthCheck
{
    private readonly ApplicationDbContext db;

    public ReadinessCheck(ApplicationDbContext db)
    {
        this.db = db;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        bool databaseHealthyCheck = CheckDatabase(db);

        if (databaseHealthyCheck)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("A healthy result."));
        }

        return Task.FromResult(
            HealthCheckResult.Unhealthy("An unhealthy result from Readiness check."));
    }

    private bool CheckDatabase(ApplicationDbContext dbContext)
    {
        try
        {
            dbContext.Database.EnsureCreatedAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }
}
