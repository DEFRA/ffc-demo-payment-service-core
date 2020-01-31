using System;
using System.Collections.Generic;
using FFCDemoPaymentService.Models;

namespace FFCDemoPaymentService.Scheduling
{
    public interface IScheduleService
    {
        void CreateSchedule(string claimId, DateTime startDate);
    }
}
