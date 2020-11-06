using System;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Data;
using Microsoft.EntityFrameworkCore;

namespace FFCDemoPaymentService.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext db;
        public PaymentService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void CreatePayment(string claimId, decimal value)
        {
            Payment payment = new Payment { ClaimId = claimId, Value = value };
            try
            {
                db.Payments.AddRange(payment);
                db.SaveChanges();
                Console.WriteLine("Created payment for {0}", claimId);
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("{0} payment exists, skipping", payment.ClaimId);
            }
        }
    }
}
