namespace WeddingPlannerApplication.Models
{
    public class VendorService
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
