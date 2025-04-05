namespace WeddingPlannerApplication.Models
{
    public class VendorAvailability
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public DateTime AvailableDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
