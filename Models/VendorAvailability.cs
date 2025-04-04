namespace WeddingPlannerApplication.Models
{
    public class VendorAvailability
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public DateTime AvailableDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
