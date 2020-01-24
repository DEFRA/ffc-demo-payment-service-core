using System;
using System.Collections.Generic;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Data;
using Amqp;

namespace FFCDemoPaymentService.Scheduling
{
    class CreateSchedule : IScheduling
    {
        private readonly ApplicationDbContext _db;
        private readonly IScheduling _schedule;
        private List<Schedule> scheduleList = new List<Schedule>();
        private DateTime paymentDate = DateTime.Now.Date;
        public CreateSchedule(IScheduling schedule, ApplicationDbContext db)
        {
            _db = db;
            _schedule = schedule;
        }

        public void newSchedule(Message msg)
        {
            if(msg != null)
            {
                try 
                {
                    scheduleList = _schedule.createSchedule(scheduleList, paymentDate, 6);
                    saveSchedule(scheduleList);
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message, ex.InnerException);
                }
            }
            else
            {
                throw new Exception("No Message");
            }
        }

        private void saveSchedule(List<Schedule> scheduleList)
        {
            _db.Schedule.AddRange(scheduleList);
            _db.SaveChanges();
        }
    }
}