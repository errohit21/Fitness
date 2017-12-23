using System.ComponentModel.DataAnnotations.Schema;

namespace FLive.Web.Models
{
    public class PriceTier : Entity
    {
        public decimal Price { get; set; }
        [NotMapped]
        public string PriceString { get { return $" {Currency} {Price.ToString("0.00")}"; } }
        public string Currency { get; set; }
        public string IapKey { get; set; }
        public bool IsSubscriptionTier { get; set; } = false;

    }
}