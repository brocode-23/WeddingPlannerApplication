namespace WeddingPlannerApplication.Models
{
    public class CoupleMember
    {
        public Guid Id { get; set; }
        public Guid CoupleId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; }
    }
}
