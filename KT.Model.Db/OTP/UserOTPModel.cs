using KT.Models.DB.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KT.Models.DB.OTP
{
    public class UserOTPModel : ICloneable
    {
        [Required]
        [Key]
        public long UserOTPId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUserModel ApplicationUser { get; set; }

        [Required]
        public byte OTPPurposeTypeId { get; set; }

        [ForeignKey("OTPPurposeTypeId")]
        public OTPPurposeTypeModel OTPPurposeType { get; set; }

        [Required]
        public string OTP { get; set; }


        [Required]
        public byte OTPStateTypeId { get; set; }

        [ForeignKey("OTPStateTypeId")]
        public OTPStateTypeModel OTPStateType { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public Byte Attempt { get; set; }

        public Object Clone()
        {
            return this;
        }
    }
}