using EprojectSem3.Models;

namespace Realtors_Portal.Models
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; } // Navigation property
        public byte Status {  get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set;} 
    }
}
