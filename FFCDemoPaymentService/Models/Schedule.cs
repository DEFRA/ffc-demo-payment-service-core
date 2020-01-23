using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FFCDemoPaymentService.Models
{
    [Table("schedule")]
    public class Schedule
    {
        [Column("scheduleId")]
        public int ScheduleId { get; set; }

        [Column("claimId")]
        public string ClaimId { get; set; }

        [Column("value")]
        public decimal Decimal { get; set; }
    }
}
