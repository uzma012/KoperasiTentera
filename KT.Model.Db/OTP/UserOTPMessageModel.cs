using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT.Models.DB.OTP
{
    public class UserOTPMessageModel
    {
        [Required]
        [Key]
        public long UserOTPMessageId { get; set; }

        [Required]
        public long UserOTPId { get; set; }

        [ForeignKey("UserOTPId")]
        public UserOTPModel UserOTP { get; set; }

        [Required]
        public byte OTPChannelTypeId { get; set; }

        [ForeignKey("OTPChannelTypeId")]
        public OTPChannelTypeModel OTPChannelType { get; set; }

        [Required]
        public byte OTPMessageStateTypeId { get; set; }

        [ForeignKey("OTPMessageStateTypeId ")]
        public OTPMessageStateTypeModel OTPMessageStateType { get; set; }

        public string ExternalMessageId { get; set; }
    }
}