namespace WeddingPlannerApplication.Models
{
    public class WeddingTimeline
    {
        public Guid Id { get; set; }
        public Guid CoupleId { get; set; }
        public string EventName { get; set; }
        public DateTime EventTime { get; set; }
        public string EventDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
