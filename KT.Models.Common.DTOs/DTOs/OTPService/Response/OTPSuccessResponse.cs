using KT.Models.DB.User;

namespace KT.Models.Common.DTOs.OTPService
{
    public class OTPSuccessResponse
    {
        public Guid? UDID { get; set; }
        public string OTP { get; set; }
        public ApplicationUserModel ApplicationUser { get; set; }
        public string ExternalMessageId  { get; set; }
        public byte OTPPurposeType { get; set; }
        
    }
}
