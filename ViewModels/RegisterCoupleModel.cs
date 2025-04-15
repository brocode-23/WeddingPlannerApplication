using System.ComponentModel.DataAnnotations;

namespace WeddingPlannerApplication.ViewModels
{
    public class RegisterCoupleModel
    {
        [Required]
        public string GroomFirstName { get; set; }

        [Required]
        public string GroomLastName { get; set; }

        [Required]
        [EmailAddress]
        public string GroomEmail { get; set; }

        [Required]
        public string GroomPassword { get; set; }

        [Required]
        [Phone]
        public string GroomPhone { get; set; }

        [Required]
        public string BrideFirstName { get; set; }

        [Required]
        public string BrideLastName { get; set; }

        [Required]
        [EmailAddress]
        public string BrideEmail { get; set; }

        [Required]
        public string BridePassword { get; set; }

        [Required]
        [Phone]
        public string BridePhone { get; set; }

        [Required]
        public DateTime WeddingDate { get; set; }

        [Required]
        public decimal Budget { get; set; }
    }
}
