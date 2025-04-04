namespace WeddingPlannerApplication.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid VendorId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
