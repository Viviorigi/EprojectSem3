using System.ComponentModel.DataAnnotations;

namespace EprojectSem3.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage ="Category Name is not required")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Listing>? Listings { get; set; } 
    }

}
