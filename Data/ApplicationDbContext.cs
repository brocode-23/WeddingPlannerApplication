using WeddingPlannerApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlannerApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {


        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }  // <-- Add this

        // Vendor Management
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorCat> VendorCategories { get; set; }
        public DbSet<VendorPackage> VendorPackages { get; set; }
        public DbSet<VendorRating> VendorRatings { get; set; }


    
    }
}
