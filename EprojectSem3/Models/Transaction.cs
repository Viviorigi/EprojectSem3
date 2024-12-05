namespace EprojectSem3.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } // Navigation property
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } // Navigation property
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime TransactionDate { get; set; }
    }

}
