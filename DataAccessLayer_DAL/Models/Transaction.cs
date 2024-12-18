using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer_DAL.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }  
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public DateTime? PaymentDate { get; set; } 
        public bool IsPaid { get; set; }

    }
}
