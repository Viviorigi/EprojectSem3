using System.Reflection;

namespace EprojectSem3.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public byte Role { get; set; }
        public byte Status { get; set; }
        public int? SubscriptionId { get; set; }

        // Navigation property
        public Subscription Subscription { get; set; }
        public ICollection<Listing> Listings { get; set; }
        public ICollection<UserSubscription> UserSubscriptions { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }


}
