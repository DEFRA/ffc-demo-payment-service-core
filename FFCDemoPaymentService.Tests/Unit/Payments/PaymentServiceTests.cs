using NUnit.Framework;
using Moq;
using FFCDemoPaymentService.Payments;
using FFCDemoPaymentService.Data;
using FFCDemoPaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace FFCDemoPaymentService.Tests.Unit.Paying
{
    [TestFixture]
    public class PaymentServiceTests
    {
        string claimId;
        decimal value;
        private IPaymentService paymentService;
        Mock<ApplicationDbContext> mockContext;
        Mock<DbSet<Payment>> mockPaymentDbSet;

        [SetUp]
        public void Setup()
        {
            mockContext = new Mock<ApplicationDbContext>(new Mock<PostgresConnectionStringBuilder>().Object);
            mockPaymentDbSet = new Mock<DbSet<Payment>>();
            mockContext.Setup(x => x.Payments).Returns(mockPaymentDbSet.Object);
            paymentService = new PaymentService(mockContext.Object);
        }

        [Test]
        public void Test_CreatePayment_SavesChanges()
        {
            claimId = "ID123";
            value = 10.35m;
            paymentService.CreatePayment(claimId, value);
            mockContext.Verify(x => x.SaveChanges(), Times.AtMostOnce);
        }

        [Test]
        public void Test_CreatePayment_AddsRange()
        {
            claimId = "ID123";
            value = 10.35m;
            paymentService.CreatePayment(claimId, value);
            mockContext.Verify(x => x.AddRange(It.IsAny<Payment>()), Times.AtMostOnce);
        }
    }
}
