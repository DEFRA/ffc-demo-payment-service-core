using System;
using System.Collections.Generic;
using FFCDemoPaymentService.Models;
using Amqp;

namespace FFCDemoPaymentService.Scheduling
{
    public interface IScheduleService
    {
        void CreateSchedule(string claimId, DateTime startDate, int scheduleLength);
    }
}