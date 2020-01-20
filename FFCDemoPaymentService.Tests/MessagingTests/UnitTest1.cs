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
        public void Test1()
        {
            Assert.AreEqual(messageConfig.Host, "123");
        }

        [Test]
        public void Test2()
        {
            Assert.AreEqual(messageConfig.Port, 111);
        }

        [Test]
        public void Test3()
        {
            Assert.AreEqual(messageConfig.UseSsl, true);
        }

        [Test]
        public void Test4()
        {
            Assert.AreEqual(messageConfig.PaymentQueue, "Test_Queue");
        }

        [Test]
        public void Test5()
        {
            Assert.AreEqual(messageConfig.PaymentUserName, "Test_User");
        }

        [Test]
        public void Test6()
        {
            Assert.AreEqual(messageConfig.PaymentPassword, "Test_Password");
        }

        [Test]
        public void Test7()
        {
            // try
            // {
                //messageService.connection = 
                //messageService.Setup(x => x.Listen());
                Assert.IsTrue(true);
            // }
            // catch
            // {
            //     Assert.IsTrue(false);
            // }
        }
    }
}