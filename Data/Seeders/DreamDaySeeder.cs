using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.Data.Seeders
{
    public static class DreamDaySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "Admin", "Planner", "Couple", "Vendor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var rnd = new Random();
            var users = new List<ApplicationUser>();

            if (!context.Users.Any())
            {
                for (int i = 0; i < 20; i++)
                {
                    var role = roles[i % roles.Length];
                    var email = $"user{i}@mail.com";

                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        FirstName = $"First{i}",
                        LastName = $"Last{i}",
                        Role = role,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        ProfilePicture = ""
                    };

                    var result = await userManager.CreateAsync(user, "Password@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);
                        users.Add(user);
                    }
                }

                await context.SaveChangesAsync();
            }

            users = await context.Users.OfType<ApplicationUser>().ToListAsync();

            var coupleUsers = users.Where(u => u.Role == "Couple").Take(10).ToList();
            var plannerUsers = users.Where(u => u.Role == "Planner").ToList();
            var vendorUsers = users.Where(u => u.Role == "Vendor").ToList();

            if (!context.VendorCategories.Any())
            {
                context.VendorCategories.AddRange(
                    new VendorCat { Name = "Photographer" },
                    new VendorCat { Name = "Caterer" },
                    new VendorCat { Name = "Decorator" },
                    new VendorCat { Name = "DJ" },
                    new VendorCat { Name = "Venue" }
                );
                await context.SaveChangesAsync();
            }

            var vendorCategories = await context.VendorCategories.ToListAsync();

            if (!context.Couples.Any())
            {
                int coupleCount = coupleUsers.Count / 2;

                for (int i = 0; i < coupleCount; i++)
                {
                    var couple = new Couple
                    {
                        WeddingDate = DateTime.UtcNow.AddMonths(i + 1),
                        Budget = 20000 + (i * 1000),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.Couples.Add(couple);
                    await context.SaveChangesAsync();

                    var groom = coupleUsers[i * 2];
                    var bride = coupleUsers[i * 2 + 1];

                    context.CoupleMembers.AddRange(
                        new CoupleMember
                        {
                            CoupleId = couple.Id,
                            UserId = groom.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        },
                        new CoupleMember
                        {
                            CoupleId = couple.Id,
                            UserId = bride.Id,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        });

                    context.WeddingChecklists.Add(new WeddingChecklist
                    {
                        CoupleId = couple.Id,
                        TaskName = "Book Venue",
                        TaskDescription = "Search and finalize a venue",
                        TaskStatus = "Incomplete",
                        DueDate = DateTime.UtcNow.AddMonths(2),
                        AssignedTo = "both",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });

                    context.Guests.Add(new Guest
                    {
                        CoupleId = couple.Id,
                        FirstName = $"Guest{i}",
                        LastName = "Smith",
                        Email = $"guest{i}@mail.com",
                        RSVPStatus = "Pending",
                        MealPreference = "Vegetarian",
                        Allergies = "None",
                        SeatingArrangement = "Table 1",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });

                    context.WeddingBudgets.Add(new WeddingBudget
                    {
                        CoupleId = couple.Id,
                        Category = "Catering",
                        AllocatedAmount = 5000,
                        SpentAmount = 0,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });

                    context.WeddingTimelines.Add(new WeddingTimeline
                    {
                        CoupleId = couple.Id,
                        EventName = "Ceremony",
                        EventTime = DateTime.UtcNow.AddMonths(1).AddHours(16),
                        EventDescription = "Wedding Ceremony",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });

                    var planner = plannerUsers[rnd.Next(plannerUsers.Count)];
                    context.WeddingPlanners.Add(new WeddingPlanner
                    {
                        CoupleId = couple.Id,
                        PlannerUserId = planner.Id,
                        AssignedDate = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });

                    await context.SaveChangesAsync();
                }
            }

            Console.WriteLine("Checking if any vendors exist...");
            if (!context.Vendors.Any())
            {
                Console.WriteLine("No vendors found. Seeding vendors...");
                foreach (var vendorUser in vendorUsers)
                {
                    var vendor = new Vendor
                    {
                        UserId = vendorUser.Id,
                        Name = $"{vendorUser.FirstName} Vendor",
                        Description = "Top wedding services",
                        Pricing = 2500,
                        Location = "City Center",
                        Rating = 4.5M,
                        ProfilePicture = "",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false,
                        CategoryId = vendorCategories[rnd.Next(vendorCategories.Count)].Id,
                        ContactEmail = vendorUser.Email,
                        Phone = "0771234567",
                        WebsiteUrl = "http://vendor.com"
                    };
                    context.Vendors.Add(vendor);
                    await context.SaveChangesAsync();

                    var service = new VendorService
                    {
                        VendorId = vendor.Id,
                        ServiceName = "Wedding Photography",
                        Description = "High quality photos",
                        Price = 3000,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.VendorServices.Add(service);
                    await context.SaveChangesAsync();

                    context.VendorAvailabilities.Add(new VendorAvailability
                    {
                        VendorId = vendor.Id,
                        AvailableDate = DateTime.UtcNow.Date.AddDays(15),
                        FromTime = new TimeSpan(9, 0, 0),
                        ToTime = new TimeSpan(17, 0, 0),
                        Status = "Available",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });

                    var couple = context.Couples.First();
                    var booking = new Booking
                    {
                        CoupleId = couple.Id,
                        VendorId = vendor.Id,
                        ServiceId = service.Id,
                        BookingDate = DateTime.UtcNow,
                        ServiceDetails = "Photography service",
                        TotalAmount = 3000,
                        Status = "Confirmed",
                        PaymentStatus = "Paid",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };
                    context.Bookings.Add(booking);
                    await context.SaveChangesAsync();

                    context.Payments.Add(new Payment
                    {
                        BookingId = booking.Id,
                        PaymentAmount = 3000,
                        PaymentDate = DateTime.UtcNow,
                        PaymentMethod = "Credit Card",
                        PaymentStatus = "Paid",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });

                    context.Reviews.Add(new Review
                    {
                        VendorId = vendor.Id,
                        UserId = coupleUsers.First().Id,
                        Rating = 5,
                        Comment = "Amazing service!",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });

                    context.Venues.Add(new Venue
                    {
                        VendorId = vendor.Id,
                        Address = "123 Wedding Ave",
                        Latitude = 6.9371M,
                        Longitude = 79.8734M,
                        Capacity = 200,
                        IndoorOutdoor = "Both",
                        Amenities = "WiFi, Parking, Catering",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    });

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
