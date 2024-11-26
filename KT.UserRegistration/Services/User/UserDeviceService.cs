using KT.Exceptions.API;
using KT.Exceptions;
using KT.Interfaces.Repositories;
using KT.Interfaces.Services.Common;
using KT.Models.DB.User;
using KT.Models.Registration.Device.Request;
using KT.Repositories;
using KT.Models.Registration.Device.Response;

namespace KT.Registration.Services.User
{
    public class UserDeviceService
    {
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly ISharedSecretGeneratorService _sharedSecretGeneratorService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOTPService _otpService;
        private readonly UserOTPRepository _userOTPRepository;
        private readonly IRepository<ApplicationUserModel> _applicationUserRepository;

        public UserDeviceService(ISharedSecretGeneratorService sharedSecretGeneratorService,
            IUserCredentialRepository userCredentialRepository,
            IOTPService otpService,
            UserOTPRepository userOTPRepository, IUnitOfWork unitOfWork)
        {
            _userCredentialRepository = userCredentialRepository;
            _sharedSecretGeneratorService = sharedSecretGeneratorService;
            _otpService = otpService;
            _userOTPRepository = userOTPRepository;
            _unitOfWork = unitOfWork;
            _applicationUserRepository = _unitOfWork.Repository<ApplicationUserModel>();
        }

        public async Task<bool> PostPin(PinCodeRequest pinCodeRequest)
        {
            var userCredentialModel = new UserCredentialModel { UserPIN = pinCodeRequest.UPH };
            var userCredentialUpdated = await _userCredentialRepository.UpdateUserPin(pinCodeRequest.UUID, userCredentialModel);
            if (userCredentialUpdated == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> ResetPin(string uuid)
        {
            var userCredentialUpdated = await _userCredentialRepository.RemoveUserPinAndGetUserDetails(Guid.Parse(uuid));
            if (userCredentialUpdated == null)
            {
                return false;
            }
            return true;
        }
        public async Task<BiometricResponse> PostBiometric(string uuid)
        {
            var Useruuid = Guid.Parse(uuid);
            var bpad = _sharedSecretGeneratorService.GetPad();
            var userCredentialModel = new UserCredentialModel { BiometricEnabled = true, BiometericSharedSecret = bpad };
            var userCredentialUpdated = await _userCredentialRepository.UpdateUserBiometric(Useruuid, userCredentialModel);
            if (userCredentialUpdated == null)
            {
                throw new Exception();
            }
            return new BiometricResponse { BPAD = bpad };
        }


    }
}