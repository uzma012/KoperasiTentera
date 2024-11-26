using System.ComponentModel.DataAnnotations;

namespace KT.Models.DB.OTP
{
    public class OTPPurposeTypeModel
    {
        [Required]
        [Key]
        public byte OTPPurposeTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}