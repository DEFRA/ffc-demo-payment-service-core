using System;

namespace FFCDemoPaymentService.Messaging
{
    public interface IReceiver
    {
        void StartPolling();
    }
}
