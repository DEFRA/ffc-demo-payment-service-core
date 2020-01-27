using System;
using System.Collections.Generic;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Data;

namespace FFCDemoPaymentService.Scheduling
{
    public class ScheduleService : IScheduleService
    {
        private readonly ApplicationDbContext db;
        public ScheduleService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void CreateSchedule(string claimId)
        {
            DateTime startDate = DateTime.Now.Date;

            try
            {
                db.Schedule.AddRange(ScheduleBuilder(claimId, startDate));
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        private List<Schedule> ScheduleBuilder(string claimId, DateTime startDate)
        {
            if(claimId != null && startDate != null)
            {
                List<Schedule> schedule = new List<Schedule>();
                DateTime paymentDate = startDate.Date;

                try 
                {
                    for(int i = 0; i <= 6; i++)
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