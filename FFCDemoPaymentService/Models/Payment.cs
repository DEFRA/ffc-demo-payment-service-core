using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FFCDemoPaymentService.Models
{
    [Table("payments")]
    public class Payment
    {
        [Column("claimId")]
        [Key]
        public string ClaimId { get; set; }

        [Column("value")]
        public decimal Decimal { get; set; }
    }
}
