using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer_DAL.Models
{
    public class Statistical
    {
        public int StatisticalId { get; set; }
        public int? ListingCount { get; set; }
        public int? TransactionCount { get; set; }
        public int? UserCount { get; set; }
        public decimal? PriceCount { get; set; }
        public DateTime? CreatedAt { get; set; }

    }
}
