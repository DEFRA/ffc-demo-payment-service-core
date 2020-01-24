using System;
using System.Collections.Generic;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Data;

namespace FFCDemoPaymentService.Scheduling
{
    class SchedulerService : IScheduleService
    {
        private readonly ApplicationDbContext db;
        public SchedulerService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void CreateSchedule(string claimId, DateTime startDate, int scheduleLength)
        {
            db.Schedule.AddRange(ScheduleBuilder(claimId, startDate, scheduleLength));
            db.SaveChanges();
        }

        private List<Schedule> ScheduleBuilder(string claimId, DateTime startDate, int scheduleLength)
        {
            if(claimId != null && startDate != null)
            {
                List<Schedule> schedule = new List<Schedule>();
                DateTime paymentDate = startDate.Date;

                try 
                {
                    for(int i = 0; i <= scheduleLength; i++)
                    {
                        schedule.Add(new Schedule() 
                        {
                            ClaimId = claimId,
                            PaymentDate = paymentDate
                        });
                        paymentDate.AddMonths(1);
                    }

                    return schedule;
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            else
            {
                throw new Exception("No data");
            }
        }
    }
}