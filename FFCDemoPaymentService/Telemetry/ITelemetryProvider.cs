using System;
using System.Collections.Generic;

namespace FFCDemoPaymentService.Telemetry
{
    public interface ITelemetryProvider
    {        
        void TrackDependency(string name, string typeName, string data, DateTime startTime, TimeSpan duration, bool success);
        void TrackEvent(string name);
        void TrackEvent(string name, Dictionary<string, string> properties);
        void TrackEvent(string name, Dictionary<string, string> properties, Dictionary<string, double> metrics);
        void TrackTrace(string message);
        void TrackException(Exception ex);
        void TrackMetric(string name, double value);
    }
}
