using System.ComponentModel.DataAnnotations;

namespace KT.Models.DB.OTP
{
    public class OTPMessageStateTypeModel
    {
        [Required]
        [Key]
        public byte OTPMessageStateTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}