using System;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using FFCDemoPaymentService.Scheduling;
using FFCDemoPaymentService.Data;
using FFCDemoPaymentService.Models;
using Microsoft.EntityFrameworkCore;

//This file is different

namespace FFCDemoPaymentService.Tests.Scheduling
{
    [TestFixture]
    public class ScheduleServiceTests
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
        public void Test_CreateSchedule_Adds_Range()
        {
            claimId = "ID123";
        
            scheduleService.CreateSchedule(claimId, DateTime.Now);

            mockScheduleDbSet.Verify(x => x.AddRange(It.IsAny<List<Schedule>>()), Times.AtMostOnce);
        }

        [Test]
        public void Test_CreateSchedule_SavesChanges()
        {
            claimId = "ID123";
        
            scheduleService.CreateSchedule(claimId, DateTime.Now);

            mockContext.Verify(x => x.SaveChanges(), Times.AtMostOnce);
        }
    }
}
