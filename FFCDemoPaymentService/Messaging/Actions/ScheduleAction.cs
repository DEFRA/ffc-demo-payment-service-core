using System;
using FFCDemoPaymentService.Models;
using FFCDemoPaymentService.Scheduling;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace FFCDemoPaymentService.Messaging.Actions
{
    public class ScheduleAction : IMessageAction<Schedule>, IMessageDeserializer<Claim>
    {
        readonly IServiceScopeFactory serviceScopeFactory;

        public ScheduleAction(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public void ReceiveMessage(string message)
        {
            var claim = DeserializeMessage(message);
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var scheduleService = scope.ServiceProvider.GetRequiredService<IScheduleService>();
                scheduleService.CreateSchedule(claim.ClaimId, DateTime.Now);
            }
        }

        public Claim DeserializeMessage(string message)
        {
            return JsonConvert.DeserializeObject<Claim>(message);
        }
    }
}
