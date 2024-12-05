namespace EprojectSem3.Models
{
    public class Region
    {
        public int RegionId { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<City> Cities { get; set; }
    }


}
