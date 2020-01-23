using System;
using System.ComponentModel;
using System.IO;
using NUnit.Framework;
using Moq;
using FFCDemoPaymentService;
using FFCDemoPaymentService.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;


namespace FFCDemoPaymentService.UnitTests.MessagingTests
{

    public class MessageServiceTests
    {        
        MessageConfig messageConfig;
        MessageService messageService;

        [SetUp]
        public void Setup()
        {
            messageConfig = new MessageConfig();
            messageConfig.Host = "123";
            messageConfig.Port = 111;
            messageConfig.UseSsl = true;
            messageConfig.PaymentQueue = "Test_Queue";
            messageConfig.PaymentUserName = "Test_User";
            messageConfig.PaymentPassword = "Test_Password";
            var iConnection = new Mock<IConnection>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            messageService = new MessageService ( iConnection.Object, messageConfig, serviceScopeFactory.Object );
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
        public void ConfigPassword()
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
        
        [Test]
        public void MessageServiceListen()
        {
            try
            {
                messageService.Listen();
                Assert.IsTrue(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.IsTrue(false);
            }
        }
    }
}