namespace DataAccessLayer_DAL.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public int MaxAds { get; set; }
        public bool IsAgent{ get; set; }
        public ICollection<User>? Users { get; set; } 
        public ICollection<UserSubscription>? UserSubscriptions { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }

    }

}
