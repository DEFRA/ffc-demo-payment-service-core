
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using FFCDemoPaymentService.Data;
using FFCDemoPaymentService.Messaging;
using FFCDemoPaymentService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace FFCDemoPaymentService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {


        public ServiceController()
        {

        }

        [Route("home")]
        [HttpGet]
        public Task<HealthCheckResult> CheckHealth()
        {
            try
            {
                return Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
            }
            catch
            (Exception ex)
            {
                Console.WriteLine(ex);
                return Task.FromResult(HealthCheckResult.Unhealthy("Not Healthy"));
            }

        }

    }
}
