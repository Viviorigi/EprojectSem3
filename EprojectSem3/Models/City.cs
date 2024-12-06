using System.Drawing;

namespace EprojectSem3.Models
{
    public class City
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public int RegionId { get; set; }

        // Navigation property
        public Region? Region { get; set; }
        public ICollection<Listing>? Listings { get; set; }
    }


}
