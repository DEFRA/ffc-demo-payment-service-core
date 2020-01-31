using System;
using FFCDemoPaymentService.Messaging;

namespace FFCDemoPaymentService.Messaging.Mapping
{
    public interface IMap
    {
        SqsConfig MapToSqsConfig();
    }
}
