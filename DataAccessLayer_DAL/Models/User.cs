using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DataAccessLayer_DAL.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string PhoneNumber { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public byte Role { get; set; }

        // Nullable byte, if it's null, it means no status is set
        [Range(0, 1, ErrorMessage = "Status must be either 0 (Inactive) or 1 (Active).")]
        public byte? Status { get; set; }

        public int? SubscriptionId { get; set; }
        // Navigation property
        public Subscription? Subscription { get; set; }
        public ICollection<Listing>? Listings { get; set; }
        public ICollection<UserSubscription>? UserSubscriptions { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }


}
