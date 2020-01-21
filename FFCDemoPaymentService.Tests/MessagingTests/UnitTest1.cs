using System;
using System.ComponentModel;
using System.IO;
using NUnit.Framework;
using Moq;
using FFCDemoPaymentService;
using FFCDemoPaymentService.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace FFCDemoPaymentService.UnitTests.MessagingTests
{

    public class AmqpConnectionTests
    {
        Mock<MessageService> messageService = new Mock<MessageService>();
        Mock<AmqpConnection> aConnection = new Mock<AmqpConnection>();
        MessageConfig messageConfig = new MessageConfig();


        [SetUp]
        public void Setup()
        {
            messageConfig.Host = "123";
            messageConfig.Port = 111;
            messageConfig.UseSsl = true;
            messageConfig.PaymentQueue = "Test_Queue";
            messageConfig.PaymentUserName = "Test_User";
            messageConfig.PaymentPassword = "Test_Password";
        }

        [Test]
        public void ConfigHost()
        {
            Assert.AreEqual(messageConfig.Host, "123");
        }

        [Test]
        public void ConfigPort()
        {
            Assert.AreEqual(messageConfig.Port, 111);
        }

        [Test]
        public void ConfigSSL()
        {
            Assert.AreEqual(messageConfig.UseSsl, true);
        }

        [Test]
        public void ConfigQueue()
        {
            Assert.AreEqual(messageConfig.PaymentQueue, "Test_Queue");
        }

        [Test]
        public void COnfigUsername()
        {
            Assert.AreEqual(messageConfig.PaymentUserName, "Test_User");
        }

        [Test]
        public void ConfigPAssword()
        {
            Assert.AreEqual(messageConfig.PaymentPassword, "Test_Password");
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