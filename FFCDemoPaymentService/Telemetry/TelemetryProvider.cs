using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace FFCDemoPaymentService.Telemetry
{
    public class TelemetryProvider : ITelemetryProvider
    {
        private readonly TelemetryClient client;        
        private readonly string cloudRoleName;

        public TelemetryProvider(IConfiguration configuration)
        {            
            this.client = new TelemetryClient(new TelemetryConfiguration(configuration["ApplicationInsights:InstrumentationKey"]));
            this.cloudRoleName = configuration["ApplicationInsights:CloudRole"];
        }

        public string SessionId
        {
            get => this.client.Context.Session.Id;
            set => this.client.Context.Session.Id = value;            
        }
        
        public void TrackEvent(string name)
        {
            this.client.TrackEvent($"{this.cloudRoleName} - {name}");
        }

        public void TrackEvent(string name, Dictionary<string, string> properties)
        {
            this.client.TrackEvent($"{this.cloudRoleName} - {name}", properties);
        }

        public void TrackEvent(string name, Dictionary<string, string> properties, 
                                Dictionary<string, double> metrics)
        {
            this.client.TrackEvent($"{this.cloudRoleName} - {name}", properties, metrics);
        }

        public void TrackTrace(string message)
        {
            this.client.TrackTrace($"{this.cloudRoleName} - {message}");
        }

        public void TrackMetric(string name, double value)
        {
            this.client.TrackMetric($"{this.cloudRoleName} - {name}", value);
        }

        public void TrackException(Exception ex)
        {
            this.client.TrackException(ex);
        }

        public void TrackDependency(string name, string typeName, string data, DateTime startTime, 
                                      TimeSpan duration, bool success)
        {
            this.client.TrackDependency(typeName, $"{this.cloudRoleName} - {name}", data, startTime, duration, success);
        }
    }
}
