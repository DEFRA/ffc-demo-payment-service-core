using System;
using System.Linq;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using FFCDemoPaymentService.HealthChecks;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;

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
            services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameInitializer("ffc-demo-payment-service-core"));
            services.AddApplicationInsightsTelemetry();
            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), o => o.SetPostgresVersion(9, 6)));

            var messageConfig = Configuration.GetSection("Messaging").Get<MessageConfig>();

            services.AddSingleton(messageConfig);
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddSingleton<IMessageAction<Schedule>, ScheduleAction>();
            services.AddSingleton<IMessageAction<Payment>, PaymentAction>(); 

            services.AddHealthChecks()
                .AddCheck<ReadinessCheck>("ServiceReadinessCheck")
                .AddCheck<LivenessCheck>("ServiceLivenessCheck");

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext)
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

            ApplyMigrations(dbContext);
        }

        public void ApplyMigrations(ApplicationDbContext dbContext)
        {
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                Console.WriteLine("Pending migrations found, updating database");
                try
                {
                    dbContext.Database.Migrate();
                    Console.WriteLine("Database migration complete");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error running migrations: {0}", ex);
                }                
            }
        }
    }
}
