using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using FFCDemoPaymentService.Scheduling;
using FFCDemoPaymentService.Data;
using FFCDemoPaymentService.Models;
using Microsoft.EntityFrameworkCore;

namespace FFCDemoPaymentService.UnitTests.ScheduleTests
{
    [TestFixture]
    public class ScheduleTests
    {
        string claimId;
        private IScheduleService scheduleService;
        Mock<ApplicationDbContext> mockContext;
        Mock<DbSet<Schedule>> mockScheduleDbSet;
        
        [SetUp]
        public void Setup()
        {
            mockContext = new Mock<ApplicationDbContext>();
            mockScheduleDbSet = new Mock<DbSet<Schedule>>();
            mockContext.Setup(x => x.Schedule).Returns(mockScheduleDbSet.Object);
            scheduleService = new ScheduleService(mockContext.Object);
        }

        [Test]
        public void CreateSchedule_add_range_schedule()
        {
            claimId = "ID123";
        
            scheduleService.CreateSchedule(claimId);

            mockScheduleDbSet.Verify(x => x.AddRange(It.IsAny<List<Schedule>>()), Times.AtMostOnce);

        }

        [Test]
        public void CreateSchedule_save_changes()
        {
            claimId = "ID123";
        
            scheduleService.CreateSchedule(claimId);

            mockContext.Verify(x => x.SaveChanges(), Times.AtMostOnce);

        }

    }
}