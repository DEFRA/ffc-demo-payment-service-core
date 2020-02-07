using System;
using System.Collections.Generic;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Data;

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
            Console.WriteLine("Creating Payment for {0}", claimId);
            Payment payment = new Payment{ClaimId = claimId, Value = value};
            db.Payments.AddRange(payment);
            db.SaveChanges();
        }       
    }
}
