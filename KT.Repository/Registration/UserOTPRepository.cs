

using KT.Interfaces.Repositories;
using KT.Models.Common.DTOs.OTPService;
using KT.Models.Common.OTP;
using KT.Models.DB.OTP;
using KT.Models.DB.User;
using Microsoft.EntityFrameworkCore;

namespace KT.Repositories
{
    public class UserOTPRepository
    {
        //private readonly ApplicationContext _userContext;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IRepository<UserOTPModel> _userOTPRepository;
        private readonly IRepository<UserOTPMessageModel> _userOTPMessageRepository;
        private readonly IRepository<ApplicationUserModel> _applicationUserRepository;

        public UserOTPRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userOTPRepository = _unitOfWork.Repository<UserOTPModel>();
            _userOTPMessageRepository = _unitOfWork.Repository<UserOTPMessageModel>();
            _applicationUserRepository = _unitOfWork.Repository<ApplicationUserModel>();
        }

        public void ChangeOldOTPStateWOC(UserOTPModel userOTPModel)
        {
            userOTPModel.OTPStateTypeId = (byte)OTPStateTypeEnum.Expired;
            _userOTPRepository.Update(userOTPModel);
            //await _unitOfWork.CommitAsync();
        }

        public async Task<UserOTPModel> GetRecentOTP(Guid uuid)
        {

            var oldUserOTP = await _userOTPRepository.GetAll()
                .AsNoTracking()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.UUID == uuid)
                .OrderByDescending(userOTP => userOTP.CreatedOn)
                .FirstOrDefaultAsync();
            if (oldUserOTP != null)
            {
                if (oldUserOTP.OTPStateTypeId == (byte)OTPStateTypeEnum.PendingVerification
                    || oldUserOTP.OTPStateTypeId == (byte)OTPStateTypeEnum.Expired)
                {
                    return oldUserOTP;
                }
            }
            return null;
        }


        public List<UserOTPModel> GetRecentOTPByUuid(Guid uuid)
        {

            var oldUserOTP = _userOTPRepository.GetAll()
                .AsNoTracking()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.UUID == uuid && (userOTP.ApplicationUser.UserStateTypeId < 3 ||  userOTP.ApplicationUser.UserStateTypeId > 5))
                .OrderByDescending(userOTP => userOTP.CreatedOn);
            if (oldUserOTP != null && oldUserOTP.Count()!=0)
            {
                
                    return oldUserOTP.Where(userOTP=>(userOTP.OTPStateTypeId == (byte)OTPStateTypeEnum.PendingVerification
                || userOTP.OTPStateTypeId == (byte)OTPStateTypeEnum.Expired)).ToList();
                
            }
            return null;
        }

        public  List<UserOTPModel> GetRecentOTPByEmailAndPhoneNumber(string Email, string PhoneNumber)
        {
            var oldUserOTP =  _userOTPRepository.GetAll()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.Email == Email
                &&
                userOTP.ApplicationUser.MobileNumber == PhoneNumber
                && (userOTP.ApplicationUser.UserStateTypeId < 3 ||  userOTP.ApplicationUser.UserStateTypeId > 5)
                && (userOTP.OTPStateTypeId == (byte)OTPStateTypeEnum.PendingVerification
                || userOTP.OTPStateTypeId == (byte)OTPStateTypeEnum.Expired))
                .OrderByDescending(userOTP => userOTP.CreatedOn).ToList();
            return oldUserOTP;
        }


        public List<UserOTPModel> GetUserPendingOTP(int userId)
        {
            return _userOTPRepository.GetAll().Where(r => r.UserId == userId && r.OTPStateTypeId == (byte)OTPStateTypeEnum.PendingVerification)
                .OrderByDescending(r=>r.CreatedOn).Take(5).ToList();
        }

        public async Task<UserOTPModel> CreateUserOTPWOC(OTPSuccessResponse otpSuccessResponse)
        {
            try
            {
                const byte SMSChannel = 1;
                const byte AcceptedMessageState = 1;
                var timeNow = DateTime.UtcNow;
                var userOTP = new UserOTPModel
                {
                    OTP = otpSuccessResponse.OTP,
                    ApplicationUser = otpSuccessResponse.ApplicationUser,
                    OTPPurposeTypeId = otpSuccessResponse.OTPPurposeType,
                    OTPStateTypeId = 1,
                    CreatedOn = DateTime.UtcNow,
                    Attempt = 5
                };
                await _unitOfWork.Repository<UserOTPModel>().InsertAsync(userOTP);

                var userOTPMessage = new UserOTPMessageModel
                {
                    UserOTPId = userOTP.UserOTPId,
                    OTPChannelTypeId = SMSChannel,
                    OTPMessageStateTypeId = AcceptedMessageState,
                    ExternalMessageId = otpSuccessResponse.ExternalMessageId,
                    UserOTP = userOTP,
                   
                };

                await _unitOfWork.Repository<UserOTPMessageModel>().InsertAsync(userOTPMessage);
                //await _unitOfWork.CommitAsync();
                return userOTP;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserOTPModel> CreateUserOTPWOCForNewDevice(OTPSuccessResponse otpSuccessResponse)
        {
            try
            {
                const byte SMSChannel = 1;
                const byte AcceptedMessageState = 1;
                var timeNow = DateTime.Now;
                var userOTP = new UserOTPModel
                {
                    OTP = otpSuccessResponse.OTP,
                    ApplicationUser = otpSuccessResponse.ApplicationUser,
                    OTPPurposeTypeId = 2,
                    OTPStateTypeId = 1,
                    CreatedOn = DateTime.Now,
                    Attempt = 5
                };
                await _unitOfWork.Repository<UserOTPModel>().InsertAsync(userOTP);

                var userOTPMessage = new UserOTPMessageModel
                {
                    UserOTPId = userOTP.UserOTPId,
                    OTPChannelTypeId = SMSChannel,
                    OTPMessageStateTypeId = AcceptedMessageState,
                    ExternalMessageId = otpSuccessResponse.ExternalMessageId,
                    UserOTP = userOTP
                };

                await _unitOfWork.Repository<UserOTPMessageModel>().InsertAsync(userOTPMessage);
                //await _unitOfWork.CommitAsync();
                return userOTP;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserOTPModel> CreateUserOTPForPhoneNumberVerification(OTPSuccessResponse otpSuccessResponse)
        {
            try
            {
                const byte SMSChannel = 1;
                const byte AcceptedMessageState = 1;
                var timeNow = DateTime.UtcNow;
                var userOTP = new UserOTPModel
                {
                    OTP = otpSuccessResponse.OTP,
                    ApplicationUser = otpSuccessResponse.ApplicationUser,
                    OTPPurposeTypeId = 4,
                    OTPStateTypeId = 1,
                    CreatedOn = DateTime.UtcNow,
                    Attempt = 5
                };
                await _unitOfWork.Repository<UserOTPModel>().InsertAsync(userOTP);

                var userOTPMessage = new UserOTPMessageModel
                {
                    UserOTPId = userOTP.UserOTPId,
                    OTPChannelTypeId = SMSChannel,
                    OTPMessageStateTypeId = AcceptedMessageState,
                    ExternalMessageId = otpSuccessResponse.ExternalMessageId,
                    UserOTP = userOTP
                };

                await _unitOfWork.Repository<UserOTPMessageModel>().InsertAsync(userOTPMessage);
                //await _unitOfWork.CommitAsync();
                return userOTP;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserOTPModel> CreateUserOTPForEmailVerification(OTPSuccessResponse otpSuccessResponse)
        {
            try
            {
                const byte SMSChannel = 2;
                const byte AcceptedMessageState = 1;
                var timeNow = DateTime.Now;
                var userOTP = new UserOTPModel
                {
                    OTP = otpSuccessResponse.OTP,
                    ApplicationUser = otpSuccessResponse.ApplicationUser,
                    OTPPurposeTypeId = 5,
                    OTPStateTypeId = 1,
                    CreatedOn = DateTime.UtcNow,
                    Attempt = 5
                };
                await _unitOfWork.Repository<UserOTPModel>().InsertAsync(userOTP);

                var userOTPMessage = new UserOTPMessageModel
                {
                    UserOTPId = userOTP.UserOTPId,
                    OTPChannelTypeId = SMSChannel,
                    OTPMessageStateTypeId = AcceptedMessageState,
                    ExternalMessageId = otpSuccessResponse.ExternalMessageId,
                    UserOTP = userOTP
                };

                await _unitOfWork.Repository<UserOTPMessageModel>().InsertAsync(userOTPMessage);
                //await _unitOfWork.CommitAsync();
                return userOTP;
            }
            catch
            {
                return null;
            }
        }

        public async Task<UserOTPModel> ReadUserOTPWithPendingStatus(Guid UUID)
        {
            const byte OTPStateTypePending = (byte)OTPStateTypeEnum.PendingVerification;
            var userOTPModel = await _userOTPRepository.GetAll()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.UUID == UUID &&
                userOTP.OTPStateTypeId == OTPStateTypePending)
                .OrderByDescending(userOTP => userOTP.CreatedOn)
                .FirstOrDefaultAsync();

            return userOTPModel;
        }

        public async Task<UserOTPModel> ReadUserOTPWithPendingStatusUsingEmail(string email, string phoneNumber)
        {
            const byte OTPStateTypePending = (byte)OTPStateTypeEnum.PendingVerification;
            //var applicationUserRepo = _unitOfWork.Repository<ApplicationUserModel>();
            //var userOTPRepo = _unitOfWork.Repository<UserOTPModel>();

            var userOTPModel = await _userOTPRepository.GetAll()
                //.AsNoTracking()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.Email == email &&
                userOTP.ApplicationUser.MobileNumber == phoneNumber &&
                userOTP.OTPStateTypeId == OTPStateTypePending)
                .OrderByDescending(userOTP => userOTP.CreatedOn)
                .FirstOrDefaultAsync();

            return userOTPModel;
        }

        public async Task<UserOTPModel> ReadUserOTPWithPendingStatusUsingNewPhoneNumber(string email, string phoneNumber)
        {
            const byte OTPStateTypePending = (byte)OTPStateTypeEnum.PendingVerification;
            //var applicationUserRepo = _unitOfWork.Repository<ApplicationUserModel>();
            //var userOTPRepo = _unitOfWork.Repository<UserOTPModel>();

            var userOTPModel = await _userOTPRepository.GetAll()
                //.AsNoTracking()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.Email == email &&
                userOTP.ApplicationUser.MobileNumber == phoneNumber &&
                userOTP.OTPStateTypeId == OTPStateTypePending)
                .OrderByDescending(userOTP => userOTP.CreatedOn)
                .FirstOrDefaultAsync();

            return userOTPModel;
        }

        public async Task UpdateUserOTPStatusWithRegistered(Guid UUID)
        {
            const byte OTPStatusVerified = (byte)OTPStateTypeEnum.Verified;
            const byte OTPStatusPending = (byte)OTPStateTypeEnum.PendingVerification;
            var userOTP = await _userOTPRepository.GetAll()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.UUID == UUID &&
                userOTP.OTPStateTypeId == OTPStatusPending)
                .OrderByDescending(userOTP => userOTP.CreatedOn)
                .FirstOrDefaultAsync();
            userOTP.OTPStateTypeId = OTPStatusVerified;
            _userOTPRepository.Update(userOTP);
            _unitOfWork.Commit();
        }

        public async Task UpdateUserOTPStatusWithExpired(Guid UUID)
        {
            const byte OTPExpires = (byte)OTPStateTypeEnum.Expired;
            const byte OTPStatusPending = (byte)OTPStateTypeEnum.PendingVerification;
            var userOTP = await _userOTPRepository.GetAll()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.UUID == UUID &&
                userOTP.OTPStateTypeId == OTPStatusPending)
                .OrderByDescending(userOTP => userOTP.CreatedOn)
                .FirstOrDefaultAsync();
            userOTP.OTPStateTypeId = OTPExpires;
            _userOTPRepository.Update(userOTP);
            _unitOfWork.Commit();
        }

        public async Task UpdateUserOTPStatusWithRegisteredUsingEmail(string email, string phoneNumber)
        {
            const byte OTPStatusVerified = (byte)OTPStateTypeEnum.Verified;
            const byte OTPStatusPending = (byte)OTPStateTypeEnum.PendingVerification;
            //var users = _applicationUserRepository.GetAll().AsNoTracking()
            //    .Where(user => user.UUID == UUID);
            var userOTP = await _userOTPRepository.GetAll()
                //.AsNoTracking()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.Email == email &&
                userOTP.ApplicationUser.MobileNumber == phoneNumber &&
                userOTP.OTPStateTypeId == OTPStatusPending)
                .OrderByDescending(userOTP => userOTP.CreatedOn)
                .FirstOrDefaultAsync();
            //var userOTPResult = await _unitOfWork.Repository<UserOTPModel>()
            //    .FindAsync(x => x.UserId == userModelResult.UserId && x.OTPStateTypeId == OTPStatusPending);
            userOTP.OTPStateTypeId = OTPStatusVerified;
            _userOTPRepository.Update(userOTP);
            _unitOfWork.Commit();
        }

        public async Task UpdateUserOTPStatusWithRegisteredUsingEmailAndNewPhoneNumber(string email, string phoneNumber)
        {
            const byte OTPStatusVerified = (byte)OTPStateTypeEnum.Verified;
            const byte OTPStatusPending = (byte)OTPStateTypeEnum.PendingVerification;
            //var users = _applicationUserRepository.GetAll().AsNoTracking()
            //    .Where(user => user.UUID == UUID);
            var userOTP = await _userOTPRepository.GetAll()
                //.AsNoTracking()
                .Include(userOTP => userOTP.ApplicationUser)
                .Where(userOTP => userOTP.ApplicationUser.Email == email &&
                userOTP.ApplicationUser.MobileNumber == phoneNumber &&
                userOTP.OTPStateTypeId == OTPStatusPending)
                .OrderByDescending(userOTP => userOTP.CreatedOn)
                .FirstOrDefaultAsync();
            userOTP.OTPStateTypeId = OTPStatusVerified;
            _userOTPRepository.Update(userOTP);
            _unitOfWork.Commit();
        }

        public async Task ResetPinUserOTP(OTPSuccessResponse otpSuccessResponse,string ip)
        {
            var userOTP = new UserOTPModel
            {
                OTP = otpSuccessResponse.OTP,
                ApplicationUser = otpSuccessResponse.ApplicationUser,
                OTPPurposeTypeId = (byte)OTPPurposeTypeEnum.PinReset,
                OTPStateTypeId = (byte)OTPStateTypeEnum.PendingVerification,
                CreatedOn = DateTime.Now,
                Attempt = 5
            };

            await _userOTPRepository.InsertAsync(userOTP);
            var userOTPMessage = new UserOTPMessageModel
            {
                UserOTPId = userOTP.UserOTPId,
                OTPChannelTypeId = (byte)OTPChannelTypeEnum.SMS,
                OTPMessageStateTypeId = (byte)OTPMessageStateTypeEnum.Accepted,
                ExternalMessageId = otpSuccessResponse.ExternalMessageId,
                UserOTP = userOTP
            };
            await _userOTPMessageRepository.InsertAsync(userOTPMessage);
            await _unitOfWork.CommitAsync();
        }

        public void UpdateUserOTPAttempts(UserOTPModel userOTPModel)
        {
            _userOTPRepository.Update(userOTPModel);
            _unitOfWork.Commit();
        }

        public void UpdateApplicationUserState(Guid uuid, byte state)
        {
            var applicationUser = _applicationUserRepository.GetAll().Where(r => r.UUID == uuid).FirstOrDefault();
            applicationUser.UserStateTypeId = state;
            _applicationUserRepository.Update(applicationUser);
            _unitOfWork.Commit();
        }
    }
}