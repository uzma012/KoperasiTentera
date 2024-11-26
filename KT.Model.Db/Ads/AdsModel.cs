using System.ComponentModel.DataAnnotations;

namespace KT.Models.DB.Ads
{
    public class AdsModel
    {
        [Key]
        public int AdId { get; set; } // Unique identifier for the ad

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters.")]
        public string Title { get; set; } // Title of the ad

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } // Description of the ad

        [Required(ErrorMessage = "Target URL is required.")]
        [Url(ErrorMessage = "Invalid URL format.")]
        public string TargetUrl { get; set; } // URL to redirect when clicked

        [Url(ErrorMessage = "Invalid Image URL format.")]
        public string ImageUrl { get; set; } // URL of the ad image

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; } // Start date and time of the ad

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; } // End date and time of the ad

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp of creation

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
