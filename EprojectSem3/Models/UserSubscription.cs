namespace EprojectSem3.Models
{
    public class UserSubscription
    {
        public int UserSubscriptionId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } // Navigation property
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } // Navigation property
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
