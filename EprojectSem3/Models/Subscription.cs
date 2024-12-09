namespace EprojectSem3.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public int MaxAds { get; set; }
        public bool IsPriorityAd { get; set; }

        public ICollection<User>? Users { get; set; } // Liên kết đến Users
        public ICollection<UserSubscription>? UserSubscriptions { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }

}
