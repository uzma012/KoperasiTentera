using KT.Interfaces.Repositories;
using KT.Models.DB.User;
using KT.Models.Registration.Registration.Request;
using Microsoft.EntityFrameworkCore;

namespace KT.Repositories
{
    public class UserCredentialRepository : IUserCredentialRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ApplicationUserModel> _applicationUserRepository;
        private readonly IRepository<UserDeviceModel> _userDeviceRepository;
        private readonly IRepository<UserCredentialModel> _userCredentialRepository;

        public UserCredentialRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _applicationUserRepository = _unitOfWork.Repository<ApplicationUserModel>();
            _userDeviceRepository = _unitOfWork.Repository<UserDeviceModel>();
            _userCredentialRepository = _unitOfWork.Repository<UserCredentialModel>();
        }


        public async Task<string> CreateOrUpdateUserCredential(OTPRequest otp, UserCredentialModel userCredentialModel)
        {
            IQueryable<UserCredentialModel> userCredential;
            UserDeviceModel userDevice;
            if (otp.UUID != null)
            {
                userDevice = await ReadUserDeviceByUUID(Guid.Parse(otp.UUID)).FirstOrDefaultAsync();
                userCredential = _userCredentialRepository.GetAll()
                   .Include(userCredential => userCredential.UserDevice)
                   .ThenInclude(userDevice => userDevice.ApplicationUser)
                   .Where(userCredential => userCredential.UserDevice.ApplicationUser.UUID == Guid.Parse(otp.UUID) && userCredential.UserDeviceId == userDevice.UserDeviceId);
            }
            else
            {
                userDevice = await ReadUserDeviceByEmailAndPhoneNumber(otp.Email, otp.PhoneNumber).FirstOrDefaultAsync();
                userCredential = _userCredentialRepository.GetAll()
                   .Include(userCredential => userCredential.UserDevice)
                   .ThenInclude(userDevice => userDevice.ApplicationUser)
                   .Where(userCredential => userCredential.UserDevice.ApplicationUser.Email == otp.Email &&
                   userCredential.UserDevice.ApplicationUser.MobileNumber == otp.PhoneNumber && userCredential.UserDeviceId == userDevice.UserDeviceId);
            }
            if (await userCredential.AnyAsync())
            {
                // This means that user credential already exists
                // The user is coming here after resetting their pin
                var userCredentialModelFound = await userCredential.FirstOrDefaultAsync();
                userCredentialModelFound.SharedSecret = userCredentialModel.SharedSecret;
                _userCredentialRepository.Update(userCredentialModelFound);
            }
            else
            {
                userCredentialModel.UserDeviceId = userDevice.UserDeviceId;
                userCredentialModel.RetryCounter = 5;
                await _userCredentialRepository.InsertAsync(userCredentialModel);
            }
            await _unitOfWork.CommitAsync();
            return userDevice.ApplicationUser.UUID.ToString();
        }

        private IQueryable<UserDeviceModel> ReadUserDeviceByUUID(Guid uuid)
        {
            return _applicationUserRepository.FindBy(user => user.UUID == uuid)
                    .Join(_userDeviceRepository.GetAll()
                    .Include(ud => ud.ApplicationUser), user => user.UserId,
                    userDevice => userDevice.UserId, (user, userDevice) => userDevice);
        }

        private IQueryable<UserDeviceModel> ReadUserDeviceByEmailAndPhoneNumber(string email, string phoneNumber)
        {
            return _applicationUserRepository.FindBy(user => user.Email == email && user.MobileNumber == phoneNumber)
                    .Join(_userDeviceRepository.GetAll()
                    .Include(ud => ud.ApplicationUser), user => user.UserId,
                    userDevice => userDevice.UserId, (user, userDevice) => userDevice);
        }

        public IQueryable<UserCredentialModel> ReadUserCredentialByUUID(Guid uuid)
        {
            var userDeviceFound = ReadUserDeviceByUUID(uuid);
            return userDeviceFound
                .Join(_userCredentialRepository.GetAll(), userDevice => userDevice.UserDeviceId,
                userCredential => userCredential.UserDeviceId, (userDevice, userCredential) => userCredential);
        }

        public async Task<UserCredentialModel> RemoveUserPinAndGetUserDetails(Guid uuid)
        {

            var userCredential = (from credential in _userCredentialRepository.GetAll()
                                  join userdevice in _userDeviceRepository.GetAll()
                                  .Include(r => r.ApplicationUser)
                              on credential.UserDeviceId equals userdevice.UserDeviceId
                                  join applicationuser in _applicationUserRepository.GetAll()
                                  on userdevice.UserId equals applicationuser.UserId
                                  where applicationuser.UUID==uuid
                                  select new UserCredentialModel
                                  {
                                      BiometericSharedSecret = credential.BiometericSharedSecret,
                                      SharedSecret = credential.SharedSecret,
                                      BiometricEnabled = credential.BiometricEnabled,

                                      UserCredentialId = credential.UserCredentialId,
                                      UserDeviceId = credential.UserDeviceId,
                                      UserPIN = credential.UserPIN,
                                      UserDevice = userdevice,
                                      RetryCounter = 5
                                  }).FirstOrDefault();

            userCredential.UserPIN = null;

            _userCredentialRepository.Update(userCredential);
            await _unitOfWork.CommitAsync();
            return userCredential;
        }

        public async Task<UserCredentialModel> UpdateUserPin(Guid uuid, UserCredentialModel userCredentialModel)
        {
            var userCredentialFound = await ReadUserCredentialByUUID(uuid).FirstOrDefaultAsync();
            userCredentialFound.RetryCounter = 5;
            userCredentialFound.UserPIN = userCredentialModel.UserPIN;
            _userCredentialRepository.Update(userCredentialFound);
            await _unitOfWork.CommitAsync();
            return userCredentialFound;
        }

        public async Task<UserCredentialModel> UpdateUserBiometric(Guid uuid, UserCredentialModel userCredentialModel)
        {
            var userCredentialFound = await ReadUserCredentialByUUID(uuid).FirstOrDefaultAsync();
            userCredentialFound.BiometricEnabled = userCredentialModel.BiometricEnabled;
            userCredentialFound.BiometericSharedSecret = userCredentialModel.BiometericSharedSecret;
            _userCredentialRepository.Update(userCredentialFound);
            _unitOfWork.Commit();
            return userCredentialFound;
        }

        

        public void UpdateUserCredential(UserCredentialModel userCredentialModel)
        {
            _userCredentialRepository.Update(userCredentialModel);
        }
    }
}