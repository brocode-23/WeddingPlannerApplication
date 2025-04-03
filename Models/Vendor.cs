using Microsoft.AspNetCore.Identity;

namespace DreamDayWeddingPlanner.Models
{
    public class Vendor
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string ContactEmail { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string? WebsiteUrl { get; set; }

        public VendorCat? Category { get; set; }
        public ICollection<VendorPackage>? Packages { get; set; }
        public ICollection<VendorRating>? Ratings { get; set; }


    }


}