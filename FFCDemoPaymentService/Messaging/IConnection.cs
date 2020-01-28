using System;
using System.Threading.Tasks;

namespace FFCDemoPaymentService.Messaging
{
    public interface IConnection
    {
        Task Listen();
    }
}
