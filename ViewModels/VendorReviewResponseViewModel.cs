using WeddingPlannerApplication.Models;

namespace WeddingPlannerApplication.ViewModels
{
    public class VendorReviewResponseViewModel
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
