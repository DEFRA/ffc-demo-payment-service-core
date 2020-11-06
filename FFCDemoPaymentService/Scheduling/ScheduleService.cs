using System;
using System.Collections.Generic;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Data;
using Microsoft.EntityFrameworkCore;

namespace FFCDemoPaymentService.Scheduling
{
    public class ScheduleService : IScheduleService
    {
        private readonly ApplicationDbContext db;
        public ScheduleService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void CreateSchedule(string claimId, DateTime startDate)
        {
            try
            {
                db.Schedule.AddRange(ScheduleBuilder(claimId, startDate));
                db.SaveChanges();
                Console.WriteLine("Created schedule for {0}", claimId);
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("{0} schedule exists, skipping", claimId);
            }
        }

        private List<Schedule> ScheduleBuilder(string claimId, DateTime startDate)
        {
            List<Schedule> schedule = new List<Schedule>();
            DateTime paymentDate = startDate.Date;

            for (int i = 0; i <= 6; i++)
            {
                schedule.Add(new Schedule()
                {
                    ClaimId = claimId,
                    PaymentDate = paymentDate
                });
                paymentDate = paymentDate.AddMonths(1);
            }

            return schedule;
        }
    }
}
