using System.ComponentModel.DataAnnotations;

namespace KT.Models.DB.OTP
{
    public class OTPStateTypeModel
    {
        [Required]
        [Key]
        public byte OTPStateTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}