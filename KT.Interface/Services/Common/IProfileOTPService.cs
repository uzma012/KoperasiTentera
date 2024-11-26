using KT.Models.Common.DTOs.OTPService;
using KT.Models.DB.User;
using System;
using System.Threading.Tasks;

namespace KT.Interfaces.Services.Common
{
    public interface IProfileOTPService
    {
        public OTPSuccessResponse CreateAndSendOTPViaEmail(ApplicationUserModel applicationUser, string email);

        public Task<OTPSuccessResponse> CreateAndSendOTPAsync(ApplicationUserModel applicationUser, string mobileNumber);

        public Task<OTPVerificationResponse> VerifyOTP(string otp, Guid UUID);
    }
}