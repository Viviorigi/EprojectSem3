﻿using System.ComponentModel.DataAnnotations;

namespace EprojectSem3.Models
{
    public class Listing
    {
        public int ListingId { get; set; }
        [Required(ErrorMessage ="Title is not required")]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage ="Price is not required")]
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } // Navigation property
        public int UserId { get; set; }
        public User User { get; set; } // Navigation property
        public int CityId { get; set; }
        public City City { get; set; } // Navigation property
        public byte Status { get; set; }
        public byte Priority { get; set; } = 0;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Image>? Images { get; set; } // Liên kết đến Images
    }

}
