using System;

namespace FFCDemoPaymentService.Scheduling
{
    public interface IScheduleService
    {
        void CreateSchedule(string claimId, DateTime startDate);
    }
}
