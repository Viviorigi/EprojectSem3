namespace DataAccessLayer_DAL.Models
{
    public class Region
    {
        public int RegionId { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<City>? Cities { get; set; }
    }


}
