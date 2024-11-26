using Microsoft.EntityFrameworkCore;
using KT.Interfaces.Repositories;
using KT.Interfaces.Services.Common;
using KT.Models.Common.DTOs.OTPService;
using KT.Models.DB.OTP;
using KT.Models.DB.User;
using KT.Models.Registration.Device.Request;
using KT.Models.Registration.Registration.Request;
using KT.Models.Registration.Registration.Response;
using System.Data.Common;
using KT.Exceptions.API;
using KT.Exceptions;
using KT.Models.Common.OTP;
using KT.Repositories;

namespace KT.Registration.Services.Registration
{
    public class RegistrationService
    {
        private readonly IOTPService _OTPService;
        private readonly ITokenGeneratorService _tokenGeneratorService;
        private readonly ISharedSecretGeneratorService _sharedSecretGeneratorService;
        private readonly UserCredentialRepository _userCredentialRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserOTPRepository _userOTPRepository;
        private readonly AccountRepository _accountRepository;
        private readonly IRepository<ApplicationUserModel> _applicationUserRepository;

        public RegistrationService(IOTPService otpService,
            ITokenGeneratorService tokenGeneratorService,
            ISharedSecretGeneratorService sharedSecretGeneratorService,
             UserCredentialRepository userCredentialRepository,
             IUnitOfWork unitOfWork, UserOTPRepository userOTPRepository,
             AccountRepository accountRepository)
        {
            //_userRepository = userRepository;
            _OTPService = otpService;
            _tokenGeneratorService = tokenGeneratorService;
            _sharedSecretGeneratorService = sharedSecretGeneratorService;
            _userOTPRepository = userOTPRepository;
            _userCredentialRepository = userCredentialRepository;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _applicationUserRepository = _unitOfWork.Repository<ApplicationUserModel>();
        }


        public async Task<PreliminaryRegistrationResponse> PostRegistrationDetails(RegistrationRequest registrationRequest)
        {
            var userModel = new ApplicationUserModel
            {
                Email = registrationRequest.Email,
                FirstName = registrationRequest.FirstName.Trim(),
                LastName = registrationRequest.LastName.Trim(),
                MobileNumber = registrationRequest.PhoneNumber,
                UserStateTypeId = 1,
                ICNumber = registrationRequest.ICNumber,
                UUID = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                CreatedBy = 1,
                LastModifiedBy=1,
                LastModifiedOn=DateTime.Now

            };
            try
            {
                ApplicationUserModel userModelGenerated = new ApplicationUserModel();
                var unverifiedUser = _unitOfWork.Repository<ApplicationUserModel>().GetAll()
                    .Where(r => (r.Email == registrationRequest.Email || r.MobileNumber == registrationRequest.PhoneNumber) && r.UserStateTypeId == 1).FirstOrDefault();
                if (unverifiedUser != null)
                {
                    var pendingOTPList = _userOTPRepository.GetUserPendingOTP(unverifiedUser.UserId);
                    if (pendingOTPList.All(r => r.CreatedOn >= DateTime.UtcNow.AddHours(-6) && r.CreatedOn <= DateTime.UtcNow) && pendingOTPList.Count == 5)
                    {
                        unverifiedUser.LastModifiedOn = DateTime.UtcNow;
                        _unitOfWork.Repository<ApplicationUserModel>().Update(unverifiedUser);
                        await _unitOfWork.CommitAsync();
                        throw new Exception("Bad Request");
                    }
                    unverifiedUser.Email = registrationRequest.Email;
                    unverifiedUser.FirstName = registrationRequest.FirstName.Trim();
                    unverifiedUser.LastName = registrationRequest.LastName.Trim();
                    unverifiedUser.MobileNumber = registrationRequest.PhoneNumber;
                    unverifiedUser.UserStateTypeId = 1;
                    unverifiedUser.ICNumber=registrationRequest.ICNumber;
                    _unitOfWork.Repository<ApplicationUserModel>().Update(unverifiedUser);
                    userModelGenerated = unverifiedUser;
                    var userDevicePrevious = _unitOfWork.Repository<UserDeviceModel>().GetAll().Where(r => r.UserId == userModelGenerated.UserId).FirstOrDefault();
                    userDevicePrevious.ApplicationUser = userModelGenerated;
                    _unitOfWork.Repository<UserDeviceModel>().Update(userDevicePrevious);

                    await _unitOfWork.CommitAsync();
                    var accountId = _accountRepository.GetAccountByUserId(userModelGenerated.UserId).AccountId;
                    var otpSuccessResponse = await _OTPService.CreateAndSendOTPAsync(userModelGenerated, userModelGenerated.MobileNumber);
                    otpSuccessResponse.OTPPurposeType = 1;
                    await _userOTPRepository.CreateUserOTPWOC(otpSuccessResponse);
                    await _unitOfWork.CommitAsync();

                    return new PreliminaryRegistrationResponse
                    {
                        UUID = userModelGenerated.UUID,
                        AccountId = accountId
                    };
                }
                else
                {
                    userModelGenerated = await _unitOfWork.Repository<ApplicationUserModel>().InsertAsync(userModel);
                    var accountUserAccount = await _accountRepository.CreateAccountWOC(userModelGenerated);
                    var userDevice = new UserDeviceModel { ApplicationUser = userModelGenerated };
                    var userDeviceForResponse = await _unitOfWork.Repository<UserDeviceModel>().InsertAsync(userDevice);
                    await _unitOfWork.CommitAsync();
                    var otpSuccessResponse = await _OTPService.CreateAndSendOTPAsync(userModelGenerated, userModelGenerated.MobileNumber);
                    otpSuccessResponse.OTPPurposeType = 1;
                    await _userOTPRepository.CreateUserOTPWOC(otpSuccessResponse);
                    await _unitOfWork.CommitAsync();

                    return new PreliminaryRegistrationResponse
                    {
                        UUID = userModelGenerated.UUID,
                        AccountId = accountUserAccount.AccountId
                    };
                }
            }
            catch (Twilio.Exceptions.ApiException e)
            {
                throw new Exception("The mobile number you have input is invalid");
            }
            catch (Twilio.Exceptions.ApiConnectionException e)
            {
                throw new Exception("We were unable to send verification code to your mobile number. Please try again later");
            }
            catch (Exception e)
            {
                if (e is DbUpdateException)
                {
                    var userStateTypeForResponse = await _applicationUserRepository.GetAll().FirstOrDefaultAsync(x => x.Email == registrationRequest.Email && x.MobileNumber == registrationRequest.PhoneNumber);
                    if (userStateTypeForResponse != null)
                    {
                        if (userStateTypeForResponse.UserStateTypeId == 1)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        throw new Exception($"This mobile number or email has already been registered");
                    }

                    throw new Exception("This mobile number or email has already been registered");
                }
                throw new Exception("Something went wrong. We assure you that we will promptly fix it. Please try again later");
            }
        }


        public async Task<PreliminaryRegistrationResponse> Login (LoginRequest loginRequest)
        {
            
            try
            {
                ApplicationUserModel userModelGenerated = new ApplicationUserModel();
                var user = _unitOfWork.Repository<ApplicationUserModel>().GetAll()
                    .Where(r => (r.ICNumber == loginRequest.ICNumber)).FirstOrDefault();
                if (user != null)
                {
              
                    var otpSuccessResponse = await _OTPService.CreateAndSendOTPAsync(user, user.MobileNumber);
                    otpSuccessResponse.OTPPurposeType = 3;
                    await _userOTPRepository.CreateUserOTPWOC(otpSuccessResponse);
                    await _unitOfWork.CommitAsync();
                    var userAccount = _accountRepository.GetAccountByUserId(user.UserId);
                    return new PreliminaryRegistrationResponse
                    {
                        UUID = user.UUID,
                        AccountId = userAccount.AccountId
                    };
                }
                else
                {
                    throw new Exception($"User does not exist");
                }
            }
            catch (Twilio.Exceptions.ApiException e)
            {
                throw new Exception("The number you have input is invalid");
            }
            catch (Twilio.Exceptions.ApiConnectionException e)
            {
                throw new Exception("We were unable to send verification code to your mobile number. Please try again later");
            }
            catch (Exception e)
            {
                if (e is DbUpdateException)
                {
                    var userStateTypeForResponse = await _applicationUserRepository.GetAll().FirstOrDefaultAsync(x => x.ICNumber==loginRequest.ICNumber);
                    if (userStateTypeForResponse != null)
                    {
                        if (userStateTypeForResponse.UserStateTypeId == 1)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        throw new Exception($"User not exist");
                    }
                    throw new Exception($"User not exist");
                }
                throw new Exception("Something went wrong. We assure you that we will promptly fix it. Please try again later");
            }
        }

        public async Task<AccessTokenResponse> VerifyOTP(OTPRequest otp)
        {
            OTPVerificationResponse otpVerificationResponse;
            ApplicationUserModel userModelGenerated = null;
            if (otp.UUID != null)
            {
                Guid GuidUserID = Guid.Parse(otp.UUID);
                userModelGenerated = _unitOfWork.Repository<ApplicationUserModel>().FindBy(r => r.UUID == GuidUserID).FirstOrDefault();
            }
            else
            {
                userModelGenerated = _unitOfWork.Repository<ApplicationUserModel>().FindBy(r => r.MobileNumber == otp.PhoneNumber).FirstOrDefault();
            }
            if (userModelGenerated == null)
            {
                throw new ForbiddenException();
            }
            if (userModelGenerated.UserStateTypeId > 3)
            {
                throw new OperationCanceledException();
            }
            if (otp.UUID != null)
            {
                otpVerificationResponse = await _OTPService.VerifyOTP(otp.Otp, Guid.Parse(otp.UUID));
            }
            else
            {
                otpVerificationResponse = await _OTPService.VerifyOTPThroughEmailAndPhoneNumber(otp.Otp, otp.Email, otp.PhoneNumber);
            }
            if (otpVerificationResponse.OTPVerificationStatus == OTPVerificationResponse.OTPStatus.Correct)
            {
                var pad = _sharedSecretGeneratorService.GetPad();
                var userCredentialModel = new UserCredentialModel
                {
                    SharedSecret = pad
                };
                var uuid = await _userCredentialRepository.CreateOrUpdateUserCredential(otp, userCredentialModel);

                var userAccount = _accountRepository.GetAccountByUserId(userModelGenerated.UserId);
                var account = userAccount.AccountId;

                return new AccessTokenResponse
                {
                    AccessToken = _tokenGeneratorService.GenerateJwtToken(uuid, userAccount),
                    Pad = pad,
                    UUID = uuid
                };
            }
            else if (otpVerificationResponse.AttempsRemaining != null)
            {
                throw new ArgumentException(otpVerificationResponse.AttempsRemaining.ToString());
            }
            return null;
        }

        public async Task<bool> ResendOTP(ResendOTPRequest resendOTPRequest)
        {
            UserOTPModel oldUserOTP;
            byte oTPPurpose = 1;
            List<UserOTPModel> userOTPList = new List<UserOTPModel>();
            if (resendOTPRequest.UUID != null)
            {
                var uuid = Guid.Parse(resendOTPRequest.UUID);
                 userOTPList = _userOTPRepository.GetRecentOTPByUuid(uuid);
                oldUserOTP = userOTPList.FirstOrDefault();
                if (oldUserOTP == null)
                {
                    return false;
                }
            }
            else
            {
                userOTPList =  _userOTPRepository.GetRecentOTPByEmailAndPhoneNumber(resendOTPRequest.Email, resendOTPRequest.PhoneNumber);
                oldUserOTP = userOTPList.FirstOrDefault();
                if (oldUserOTP == null)
                {
                    throw new ForbiddenException();
                }
                else
                {
                    oTPPurpose = 2;
                }
            }
            var pendingOTP = userOTPList.Where(r => r.OTPStateTypeId == (byte)OTPStateTypeEnum.PendingVerification).OrderByDescending(r => r.CreatedOn).Take(5).ToList();
            // Changed from 2 hours to 6 hours
            if (pendingOTP.All(r => r.CreatedOn >= DateTime.UtcNow.AddHours(-6) && r.CreatedOn <= DateTime.UtcNow) && pendingOTP.Count == 5)
            {
                oldUserOTP.ApplicationUser.LastModifiedOn = DateTime.UtcNow;
                _unitOfWork.Repository<ApplicationUserModel>().Update(oldUserOTP.ApplicationUser);
                await _unitOfWork.CommitAsync();
                throw new Exception("Bad Request");
            }

            if (oldUserOTP.ApplicationUser.UserStateTypeId > 3)
            {
                throw new OperationCanceledException();
            }
            var userDevice = _unitOfWork.Repository<UserDeviceModel>().GetAll().Where(r => r.UDID == Guid.Parse(resendOTPRequest.UDID)
            && r.UserId == oldUserOTP.UserId).FirstOrDefault();
            if (userDevice == null)
            {
                throw new NonConnectivityException();
            }
            var isOlderThanAMinute = DateTime.Now.Subtract(oldUserOTP.CreatedOn).TotalSeconds > 180;
            var isPendingOTP = oldUserOTP.OTPStateTypeId == (byte)OTPStateTypeEnum.PendingVerification;

            try
            {
                if (isOlderThanAMinute)
                {
                    if (isPendingOTP)
                    {
                        _userOTPRepository.ChangeOldOTPStateWOC(oldUserOTP);
                    }
                    var otpSuccessResponse = await _OTPService.CreateAndSendOTPAsync(oldUserOTP.ApplicationUser, oldUserOTP.ApplicationUser.MobileNumber);
                    otpSuccessResponse.OTPPurposeType = oTPPurpose;
                    await _userOTPRepository.CreateUserOTPWOC(otpSuccessResponse);
                    await _unitOfWork.CommitAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Twilio.Exceptions.ApiException e)
            {
                throw new OTPException("The mobile number you have input is invalid");
            }
            catch (Twilio.Exceptions.ApiConnectionException e)
            {
                throw new OTPException("We were unable to send verification code to your mobile number. Please try again later");
            }
        }
    }
}