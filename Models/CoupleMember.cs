namespace WeddingPlannerApplication.Models
{
    public class CoupleMember
    {
        public int Id { get; set; }
        public int CoupleId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; }
    }
}
