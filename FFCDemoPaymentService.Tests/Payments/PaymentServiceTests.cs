using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using FFCDemoPaymentService.Payments;
using FFCDemoPaymentService.Data;
using FFCDemoPaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace FFCDemoPaymentService.Tests.Paying
{
    [TestFixture]
    public class PaymentServiceTests
    {
        string claimId;
        decimal Value;
        private IPaymentService paymentService;
        Mock<ApplicationDbContext> mockContext;
        Mock<DbSet<Payment>> mockPaymentDbSet;
        
        [SetUp]
        public void Setup()
        {
            mockContext = new Mock<ApplicationDbContext>();
            mockPaymentDbSet = new Mock<DbSet<Payment>>();
            mockContext.Setup(x => x.Payments).Returns(mockPaymentDbSet.Object);
            paymentService = new PaymentService(mockContext.Object);
        }

        

        [Test]
        public void Test_CreatePayment_SavesChanges()
        {
            claimId = "ID123"; 
            value ="10.35";
        
            paymentService.CreatePayment(claimId, value);

            mockContext.Verify(x => x.SaveChanges(), Times.AtMostOnce);
        }
    }
}
