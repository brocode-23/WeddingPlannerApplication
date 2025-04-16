﻿namespace WeddingPlannerApplication.Models
{
    public class VendorService
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
