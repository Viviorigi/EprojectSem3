namespace EprojectSem3.Models
{
  public class Image
{
    public int ImageId { get; set; }
    public int ListingId { get; set; }
    public Listing? Listing { get; set; } 
    public string ImagePath { get; set; }
}

}
