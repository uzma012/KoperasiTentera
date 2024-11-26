using System.ComponentModel.DataAnnotations;

namespace KT.Models.DB.OTP
{
    public class OTPChannelTypeModel
    {
        [Required]
        [Key]
        public byte OTPChannelTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}