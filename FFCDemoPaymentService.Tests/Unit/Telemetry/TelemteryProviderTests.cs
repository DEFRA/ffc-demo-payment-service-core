using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using FFCDemoPaymentService.Telemetry;
using Microsoft.Extensions.Configuration;

namespace FFCDemoPaymentService.Tests.Unit.Telemetry
{
    public class TelemetryProviderTests
    {
        private Mock<IConfiguration> mockConfiguration;

        [SetUp]
        public void Setup()
        {
            mockConfiguration = new Mock<IConfiguration>();
        }

        [Test]
        public void Test_TelemetryProvider_Methods()
        {
            mockConfiguration.Setup(x => x[It.IsNotNull<string>()]).Returns("b92bcd6b-a8b3-433d-8fc5-5cdf60e974f3");

            var telemetryProvider = new TelemetryProvider(mockConfiguration.Object);

            Assert.DoesNotThrow(() => telemetryProvider.TrackEvent("Test event"));
            Assert.DoesNotThrow(() => telemetryProvider.TrackEvent("Test event", new Dictionary<string, string>() {{ "test key", "test value" }}));
            Assert.DoesNotThrow(() => telemetryProvider.TrackEvent("Test event", 
                                    new Dictionary<string, string>() {{ "test key", "test value" } }, new Dictionary<string, double>() { { "test key", 3242 }}));
            Assert.DoesNotThrow(() => telemetryProvider.TrackTrace("Test trace"));
            Assert.DoesNotThrow(() => telemetryProvider.TrackMetric("Test metric", 10));
            Assert.DoesNotThrow(() => telemetryProvider.TrackException(new Exception("Test exception")));
            Assert.DoesNotThrow(() => telemetryProvider.TrackDependency("Test dependency", "type name", "data", DateTime.Now, new TimeSpan(), true));
        }

        [Test]
        public void Test_TelemetryProvider_Verifications()
        {
            mockConfiguration.Setup(x => x[It.IsNotNull<string>()]).Returns("b92bcd6b-a8b3-433d-8fc5-5cdf60e974f3");

            new TelemetryProvider(mockConfiguration.Object);

            mockConfiguration.Verify(mock => mock[It.IsNotNull<string>()], Times.Exactly(2));
        }
    }
}
