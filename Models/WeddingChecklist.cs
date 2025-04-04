namespace WeddingPlannerApplication.Models
{
    public class WeddingChecklist
    {
        public Guid Id { get; set; }
        public Guid CoupleId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public string TaskStatus { get; set; }
        public DateTime DueDate { get; set; }
        public string AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
