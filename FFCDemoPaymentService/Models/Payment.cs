using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FFCDemoPaymentService.Models
{
    [Table("payments")]
    public class Payment
    {
        [Column("claimId")]
        [Key]
        [JsonPropertyName("claimId")]
        [JsonRequired]
        public string ClaimId { get; set; }

        [Column("value")]
        [JsonPropertyName("value")]
        [JsonRequired]
        public decimal Value { get; set; }
    }
}
