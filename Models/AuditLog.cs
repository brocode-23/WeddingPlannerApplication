namespace WeddingPlannerApplication.Models
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string EntityType { get; set; }
        public Guid EntityId { get; set; }
        public string Action { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
