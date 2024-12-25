using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer_DAL.Models
{
    public class Listing
    {
        public int ListingId { get; set; }
        [Required(ErrorMessage ="Title is not required")]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage ="Price is not required")]
        public double Price { get; set; }
        //[Required(ErrorMessage = "Picture thumbnail cannot be empty")]
		public string? Image {  get; set; }
		public int CategoryId { get; set; }
        public Category? Category { get; set; } // Navigation property
        public int UserId { get; set; }
        public User? User { get; set; } // Navigation property
        public int CityId { get; set; }
        public City? City { get; set; } // Navigation property
        public byte Status { get; set; }
        public byte Priority { get; set; } = 0;

        public byte ContactViaForm { get; set; } = 1;

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } 

        public ICollection<Image>? Images { get; set; } //  Images
        public ICollection<BookMark>? BookMarks { get; set; } //  Bookmarks
    }

}
