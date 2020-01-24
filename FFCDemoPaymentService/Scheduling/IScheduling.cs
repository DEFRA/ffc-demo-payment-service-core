using System;
using System.Collections.Generic;
using FFCDemoPaymentService.Models;
using Amqp;

namespace FFCDemoPaymentService.Scheduling
{
    public interface IScheduling
    {
        void newSchedule(Message msg);

        List<Schedule> createSchedule(List<Schedule> scheduleList, DateTime paymentDate, int scheduleLength)
        {
            for(int i = 0; i <= scheduleLength; i++)
            {
                scheduleList.Add(scheduleItem(paymentDate));
                paymentDate.AddMonths(1);
            }

            return scheduleList;
        }

        Schedule scheduleItem(DateTime paymentDate)
        {
            return (new Schedule() {
                ScheduleId = 1,
                ClaimId = "claimid",
                Decimal = 999.99m
            });
        }
    }
}