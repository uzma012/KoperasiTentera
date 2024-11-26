using Microsoft.Extensions.Options;

using KT.Models.DB.User;
using KT.Interfaces.Services.Common;
using KT.Interfaces.Repositories;
using Twilio;
using KT.Models.Common.DTOs.OTPService;
using Twilio.Rest.Api.V2010.Account;
using KT.Models.Common.Options;
using KT.Common;
using KT.Repositories;
using System.Net.Mail;
using System.Net;

namespace KT.Registration.Services.OTP
{
    public class OTPService : IOTPService
    {
        private readonly UserOTPRepository _userOTPRepository;
        private readonly IUnitOfWork _unitOfWork;
        private bool _sendOTP;
        private readonly IConfiguration _configuration;

        public OTPService(UserOTPRepository userOTPRepositry, IOptions<OTPOptions> otpOptions, IUnitOfWork unitOfWork,
             IConfiguration configuration)
        {
            _configuration = configuration;
            _userOTPRepository = userOTPRepositry;
            _unitOfWork = unitOfWork;
            _sendOTP = otpOptions.Value.SendOTP;
        }

        public async Task<OTPSuccessResponse> CreateAndSendOTPAsync(ApplicationUserModel applicationUser, string mobileNumber)
        {


            bool doNotSendOTP = false;
            string otp;
            otp = OTPGenerator.GenerateOTP(5);

            if (_sendOTP && !doNotSendOTP)
            {
                var message = await MessageResource.CreateAsync(
                    body: "Your KT one-time verification code is " + otp + ".\n\nNever share this code with anyone. Only use it on KT App.",
                    from: new Twilio.Types.PhoneNumber("KT"),
                    to: new Twilio.Types.PhoneNumber(mobileNumber)
                );

                var smtpClient = new SmtpClient("smtp.KT.com")
                {
                    Port = 587, // Typically 587 for TLS
                    Credentials = new NetworkCredential("KT@example.com", "12345"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("KT123@example.com"),
                    Subject = "Your OTP Code",
                    Body = "Your KT one-time verification code is " + otp + ".\n\nNever share this code with anyone. Only use it on KT App.",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(applicationUser.Email);

                // Send the email asynchronously
                await smtpClient.SendMailAsync(mailMessage);
                return new OTPSuccessResponse { OTP = otp, ApplicationUser = applicationUser, ExternalMessageId = message.Sid };

            }
            else
            {
                return new OTPSuccessResponse { OTP = otp, ApplicationUser = applicationUser, ExternalMessageId = Guid.NewGuid().ToString() };
            }
        }

        public async Task<OTPVerificationResponse> VerifyOTP(string otp, Guid UUID)
        {
            try
            {
                var userOTPResult = await _userOTPRepository.ReadUserOTPWithPendingStatus(UUID);
                var timeNow = DateTime.UtcNow;
                var comparisonResult = timeNow.Subtract(userOTPResult.CreatedOn);
                if (userOTPResult.Attempt <= 0 || userOTPResult.ApplicationUser.UserStateTypeId == 4)
                {
                    throw new OperationCanceledException("User account is locked");
                }
                else if (userOTPResult.OTP == otp)
                {
                    //Checking if OTP was issued within last 3 minutes
                    if (comparisonResult.TotalSeconds <= 180)
                    {
                        await _userOTPRepository.UpdateUserOTPStatusWithRegistered(UUID);
                        byte userStateId = 2;
                        _userOTPRepository.UpdateApplicationUserState(UUID, userStateId);
                        return new OTPVerificationResponse { OTPVerificationStatus = OTPVerificationResponse.OTPStatus.Correct };
                    }
                    else // OTP is Expired
                    {
                        await _userOTPRepository.UpdateUserOTPStatusWithExpired(UUID);
                        byte userStateId = 3;

                        return new OTPVerificationResponse { OTPVerificationStatus = OTPVerificationResponse.OTPStatus.Expired };
                    }
                }
                else // OTP is Incorrect
                {
                    userOTPResult.Attempt = (byte)(userOTPResult.Attempt - 1);
                    _userOTPRepository.UpdateUserOTPAttempts(userOTPResult);
                    if (userOTPResult.Attempt <= 0)
                    {
                        byte userStateTypeId = 4;
                        _userOTPRepository.UpdateApplicationUserState(UUID, userStateTypeId);
                        await _unitOfWork.CommitAsync();
                    }

                    return new OTPVerificationResponse { OTPVerificationStatus = OTPVerificationResponse.OTPStatus.Incorrect, AttempsRemaining = userOTPResult.Attempt };
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("This user has no pending OTP");
            }
        }

        public async Task<OTPVerificationResponse> VerifyOTPThroughEmailAndPhoneNumber(string otp, string email, string phoneNumber)
        {
            try
            {
                var userOTPResult = await _userOTPRepository.ReadUserOTPWithPendingStatusUsingEmail(email, phoneNumber);
                var timeNow = DateTime.Now;
                var comparisonResult = timeNow.Subtract(userOTPResult.CreatedOn);
                if (userOTPResult.Attempt <= 0 || userOTPResult.ApplicationUser.UserStateTypeId == 4)
                {
                    throw new OperationCanceledException("User account is locked");
                }
                else if (userOTPResult.OTP == otp)
                {
                    //Checking if OTP was issued within last 3 minutes
                    if (comparisonResult.TotalSeconds <= 180)
                    {
                        await _userOTPRepository.UpdateUserOTPStatusWithRegisteredUsingEmail(email, phoneNumber);
                        byte userStateId = 2;
                        _userOTPRepository.UpdateApplicationUserState(userOTPResult.ApplicationUser.UUID, userStateId);
                        return new OTPVerificationResponse { OTPVerificationStatus = OTPVerificationResponse.OTPStatus.Correct };
                    }
                    else // OTP is Expired
                    {
                        return new OTPVerificationResponse { OTPVerificationStatus = OTPVerificationResponse.OTPStatus.Expired };
                    }
                }
                else // OTP is Incorrect
                {
                    userOTPResult.Attempt = (byte)(userOTPResult.Attempt - 1);
                    _userOTPRepository.UpdateUserOTPAttempts(userOTPResult);
                    if (userOTPResult.Attempt <= 0)
                    {
                        byte userStateId = 4;
                        _userOTPRepository.UpdateApplicationUserState(userOTPResult.ApplicationUser.UUID, userStateId);
                        await _unitOfWork.CommitAsync();
                    }
                    return new OTPVerificationResponse { OTPVerificationStatus = OTPVerificationResponse.OTPStatus.Incorrect, AttempsRemaining = userOTPResult.Attempt };
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("This user has no pending OTP");
            }
        }

        public async Task<OTPVerificationResponse> VerifyOTPThroughEmailAndNewPhoneNumber(string otp, string email, string phoneNumber)
        {
            try
            {
                var userOTPResult = await _userOTPRepository.ReadUserOTPWithPendingStatusUsingNewPhoneNumber(email, phoneNumber);
                var timeNow = DateTime.Now;
                var comparisonResult = timeNow.Subtract(userOTPResult.CreatedOn);
                if (userOTPResult.Attempt <= 0 || userOTPResult.ApplicationUser.UserStateTypeId == 4)
                {
                    throw new OperationCanceledException("User account is locked");
                }
                else if (userOTPResult.OTP == otp)
                {
                    //Checking if OTP was issued within last 3 minutes
                    if (comparisonResult.TotalSeconds <= 180)
                    {
                        await _userOTPRepository.ReadUserOTPWithPendingStatusUsingNewPhoneNumber(email, phoneNumber);

                        byte userStateId = 2;
                        _userOTPRepository.UpdateApplicationUserState(userOTPResult.ApplicationUser.UUID, userStateId);
                        return new OTPVerificationResponse { OTPVerificationStatus = OTPVerificationResponse.OTPStatus.Correct };
                    }
                    else // OTP is Expired
                    {
                        return new OTPVerificationResponse { OTPVerificationStatus = OTPVerificationResponse.OTPStatus.Expired };
                    }
                }
                else // OTP is Incorrect
                {
                    userOTPResult.Attempt = (byte)(userOTPResult.Attempt - 1);
                    _userOTPRepository.UpdateUserOTPAttempts(userOTPResult);
                    if (userOTPResult.Attempt <= 0)
                    {
                        byte userStateId = 4;
                        _userOTPRepository.UpdateApplicationUserState(userOTPResult.ApplicationUser.UUID, userStateId);
                    }
                    return new OTPVerificationResponse { OTPVerificationStatus = OTPVerificationResponse.OTPStatus.Incorrect };
                }
            }
            catch (Exception e)
            {
                throw new ArgumentException("This user has no pending OTP");
            }
        }
    }
}