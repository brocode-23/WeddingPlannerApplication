using DreamDayWeddingPlanner.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WeddingPlannerApplication.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CoupleId { get; set; }
        public int VendorId { get; set; }
        public int ServiceId { get; set; }
        public DateTime BookingDate { get; set; }
        public string ServiceDetails { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        [ValidateNever]
        public Vendor Vendor { get; set; }
        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();
        public ICollection<Review>? Reviews { get; set; } = new List<Review>();
    }
}
