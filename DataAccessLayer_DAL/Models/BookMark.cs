using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer_DAL.Models
{
    public class BookMark
    {
        public int BookMarkId { get; set; }

        public int UserId { get; set; }

        public int ListingId { get; set; }

        public DateTime CreatedAt { get; set; }= DateTime.Now;

        public  User? User { get; set; }
        public  Listing? Listing { get; set; }
    }
}
