using System;
using System.Collections.Generic;
using FFCDemoPaymentService.Models;

namespace FFCDemoPaymentService.Payments
{
    public interface IPaymentService
    {
        void CreatePayment(string claimId, decimal value);
    }
}