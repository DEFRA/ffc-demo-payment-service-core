using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FFCDemoPaymentService.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using FFCDemoPaymentService.Messaging.Actions;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Payments;
using FFCDemoPaymentService.Scheduling;
using FFCDemoPaymentService.Telemetry;
using Microsoft.ApplicationInsights.Extensibility;
using static FFCDemoPaymentService.Startup;
using System;
using FFCDemoPaymentService.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Azure.Identity;
using Microsoft.FeatureManagement;
using FFCDemoPaymentService.Config;
using FFCDemoPaymentService.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var schemaConfig = builder.Configuration.GetSection("Schema").Get<SchemaConfig>();
builder.Services.AddSingleton(schemaConfig);

var isProduction = builder.Configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "production";

var connectionStringBuilder = builder.Configuration.GetSection("Postgres").Get<PostgresConnectionStringBuilder>();
connectionStringBuilder.UseCredentialChain = isProduction;
builder.Services.AddSingleton(connectionStringBuilder);
builder.Services.AddDbContext<ApplicationDbContext>();

var messageConfig = builder.Configuration.GetSection("Messaging").Get<MessageConfig>();
messageConfig.UseCredentialChain = isProduction;
builder.Services.AddSingleton(messageConfig);

builder.Services.AddSingleton(builder.Configuration.GetSection("RedisCache").Get<CacheConfig>());
builder.Services.AddSingleton(builder.Configuration.GetSection("AzureAppConfig").Get<AppConfig>());
builder.Services.AddSingleton(builder.Configuration.GetSection("AzureStorage").Get<StorageConfig>());


builder.Services
    .AddScoped<IScheduleService, ScheduleService>()
    .AddScoped<IPaymentService, PaymentService>();
builder.Services
    .AddSingleton<IMessageAction<Schedule>, ScheduleAction>()
    .AddSingleton<IMessageAction<Payment>, PaymentAction>();
builder.Services.AddSingleton<ITelemetryProvider, TelemetryProvider>();

builder.Services.AddHealthChecks()
    .AddCheck<LivenessCheck>("Live",
        tags: new[] { "Live" })
    .AddCheck<ReadinessCheck>("Ready",
        tags: new[] { "Ready" });

if (!string.IsNullOrEmpty(builder.Configuration.GetValue<string>("ApplicationInsights:ConnectionString")))
{
    string cloudRole = builder.Configuration.GetValue<string>("ApplicationInsights:CloudRole");
    builder.Services.AddSingleton<ITelemetryInitializer>(new CloudRoleNameInitializer(cloudRole));
    builder.Services.AddApplicationInsightsTelemetry();
    Console.WriteLine("App Insights Running");
}
else
{
    Console.WriteLine("App Insights Not Running!");
}


builder.Services.AddHostedService<MessageService>();

builder.Services.AddControllers();

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthorization();
app.UseStaticFiles();

app.UseHealthChecks("/healthy", new HealthCheckOptions()
{
    Predicate = check => check.Name == "ServiceReadinessCheck"
});

app.UseHealthChecks("/healthz", new HealthCheckOptions()
{
    Predicate = check => check.Name == "ServiceLivenessCheck",
});

app.UseHealthChecks("/ready", new HealthCheckOptions()
{
    Predicate = check => check.Name == "Ready",
});

app.UseHealthChecks("/live", new HealthCheckOptions()
{
    Predicate = check => check.Name == "Live",
});

app.Run();




