using System;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace FFCDemoPaymentService.Tests.Mocks
{
    public class MockServiceScopeFactory
    {
        public Mock<IServiceScopeFactory> ServiceScopeFactory { get; set; }
        public Mock<IServiceScope> ServiceScope { get; set; }
        public Mock<IServiceProvider> ServiceProvider { get; set; }

        public MockServiceScopeFactory()
        {
            SetupServiceProvider();
            SetupServiceScope();
            SetupServiceScopeFactory();
        }

        private void SetupServiceProvider()
        {
            ServiceProvider = new Mock<IServiceProvider>();
        }

        private void SetupServiceScope()
        {
            ServiceScope = new Mock<IServiceScope>();
            ServiceScope.Setup(x => x.ServiceProvider).Returns(ServiceProvider.Object);
        }

        private void SetupServiceScopeFactory()
        {
            ServiceScopeFactory = new Mock<IServiceScopeFactory>();
            ServiceScopeFactory.Setup(x => x.CreateScope()).Returns(ServiceScope.Object);
        }
    }
}
