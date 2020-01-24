using System;
using System.ComponentModel;
using System.IO;
using NUnit.Framework;
using FFCDemoPaymentService;
using FFCDemoPaymentService.Messaging;

namespace FFCDemoPaymentService.UnitTests
{
    public class BrokerUrlTests
    {        
        [SetUp]
        public void Setup()
        {
           
        }

        [Test]
        public void BrokerUrlString_NoSsl()
        {
            var brokerUrl = new BrokerUrl("host", 11111, "JohnSmith", "password", false);
            Assert.AreEqual("amqp://JohnSmith:password@host:11111", brokerUrl.ToString());
        }
        
        [Test]
        public void BrokerUrlString_WithSsl()
        {
            var brokerUrl = new BrokerUrl("host", 11111, "JohnSmith", "password", true);
            Assert.AreEqual("amqps://JohnSmith:password@host:11111", brokerUrl.ToString());
        }
    }
}