using WeddingPlannerApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlannerApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext
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

        public DbSet<Couple> Couples { get; set; }
        public DbSet<CoupleMember> CoupleMembers { get; set; }
        public DbSet<VendorService> VendorServices { get; set; }
        public DbSet<VendorAvailability> VendorAvailabilities { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WeddingChecklist> WeddingChecklists { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<WeddingBudget> WeddingBudgets { get; set; }
        public DbSet<WeddingTimeline> WeddingTimelines { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<WeddingPlanner> WeddingPlanners { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<SystemUsageLog> SystemUsageLogs { get; set; }
    }
}
