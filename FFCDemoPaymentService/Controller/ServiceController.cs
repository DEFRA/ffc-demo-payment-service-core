
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
        public async Task<HealthCheckResult> CheckHealth()
        {
            try
            {
                return HealthCheckResult.Healthy("Healthy:");
            }
            catch
            (Exception ex)
            {
                Console.WriteLine(ex);
                return HealthCheckResult.Unhealthy("Not Healthy");
            }

        }

    }
}
