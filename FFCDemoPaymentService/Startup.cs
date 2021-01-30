using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FFCDemoPaymentService.Data;
using FFCDemoPaymentService.Messaging;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Scheduling;
using FFCDemoPaymentService.Payments;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using FFCDemoPaymentService.HealthChecks;
using Microsoft.ApplicationInsights.Extensibility;
using FFCDemoPaymentService.Telemetry;

namespace FFCDemoPaymentService
{
    public partial class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddTelemetry(services);

            var schemaConfig = Configuration.GetSection("Schema").Get<SchemaConfig>();
            services.AddSingleton(schemaConfig);

            var isProduction = Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "production";

            var connectionStringBuilder = Configuration.GetSection("Postgres").Get<PostgresConnectionStringBuilder>();
            connectionStringBuilder.UseCredentialChain = isProduction;
            services.AddSingleton(connectionStringBuilder);
            services.AddDbContext<ApplicationDbContext>();

            var messageConfig = Configuration.GetSection("Messaging").Get<MessageConfig>();
            messageConfig.UseCredentialChain = isProduction;
            services.AddSingleton(messageConfig);

            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddSingleton<IMessageAction<Schedule>, ScheduleAction>();
            services.AddSingleton<IMessageAction<Payment>, PaymentAction>();
            services.AddSingleton<ITelemetryProvider, TelemetryProvider>();

            services.AddHealthChecks()
                .AddCheck<ReadinessCheck>("ServiceReadinessCheck")
                .AddCheck<LivenessCheck>("ServiceLivenessCheck");

            services.AddControllers();
        }

        private void AddTelemetry(IServiceCollection services)
        {
            if (!string.IsNullOrEmpty(Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey")))
            {
                string cloudRole = Configuration.GetValue<string>("ApplicationInsights:CloudRole");
                services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameInitializer(cloudRole));
                services.AddApplicationInsightsTelemetry();
                Console.WriteLine("App Insights Running");
            }
            else
            {
                Console.WriteLine("App Insights Not Running!");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseHealthChecks("/healthy", new HealthCheckOptions()
            {
                Predicate = check => check.Name == "ServiceReadinessCheck"
            });

            app.UseHealthChecks("/healthz", new HealthCheckOptions()
            {
                Predicate = check => check.Name == "ServiceLivenessCheck"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
