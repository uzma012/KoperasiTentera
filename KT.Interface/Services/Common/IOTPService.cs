
using KT.Models.Common.DTOs.OTPService;
using KT.Models.DB.User;

namespace KT.Interfaces.Services.Common
{
    public interface IOTPService
    {
        public Task<OTPSuccessResponse> CreateAndSendOTPAsync(ApplicationUserModel applicationUser, string mobileNumber);

        public Task<OTPVerificationResponse> VerifyOTP(string otp,Guid UUID);

        public Task<OTPVerificationResponse> VerifyOTPThroughEmailAndPhoneNumber(string otp, string email, string phoneNumber);

        public Task<OTPVerificationResponse> VerifyOTPThroughEmailAndNewPhoneNumber(string otp, string email, string phoneNumber);
    }
}