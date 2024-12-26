using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer_DAL.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public int ListingId { get; set; }
        public double RatingValue { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? User { get; set; }
        public Listing? Listing { get; set; }
    }

}
