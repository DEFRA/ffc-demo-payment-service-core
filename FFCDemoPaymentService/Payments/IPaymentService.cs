namespace FFCDemoPaymentService.Payments
{
    public interface IPaymentService
    {
        void CreatePayment(string claimId, decimal value);
    }
}
