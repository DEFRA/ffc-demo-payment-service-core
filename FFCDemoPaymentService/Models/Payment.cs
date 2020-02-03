using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace FFCDemoPaymentService.Models
{
    [Table("payments")]
    public class Payment
    {
        [Column("claimId")]
        [Key]
        [JsonProperty(PropertyName = "claimId", Required = Required.Always)]
        public string ClaimId { get; set; }

        [Column("value")]
        [JsonProperty(PropertyName = "value", Required = Required.Always)]
        public decimal Value { get; set; }
    }
}
