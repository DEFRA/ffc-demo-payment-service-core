using System;
using System.ComponentModel;
using System.IO;
using NUnit.Framework;
using Moq;
using FFCDemoPaymentService;
using FFCDemoPaymentService.Messaging;
using Microsoft.Extensions.DependencyInjection;


namespace FFCDemoPaymentService.UnitTests
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
            var connection = new Mock<IConnection>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            messageService = new MessageService(connection.Object, messageConfig, serviceScopeFactory.Object);
        }
    }
}