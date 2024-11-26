using Microsoft.EntityFrameworkCore;

using KT.Interfaces.Repositories;
using KT.Interfaces.Services.Common;
using KT.Models.Authorisation.Request;
using KT.Models.Authorisation.Response;

namespace KT.Registration.Services.User
{
    public class UserAuthoriseService
    {
        private readonly ITOTPService _totpService;
        private readonly IUserCredentialRepository _userCredentialRepository;
        private readonly ITokenGeneratorService _tokenGeneratorService;

        public UserAuthoriseService(ITOTPService totpService,
            ITokenGeneratorService tokenGeneratorService,
            IUserCredentialRepository userCredentialRepository)
        {
            _totpService = totpService;
            _tokenGeneratorService = tokenGeneratorService;
            _userCredentialRepository = userCredentialRepository;
        }

        public async Task<UserAuthorisationResponse> PostAuthorisation(UserAuthorisationRequest userAuthorisationRequest)
        {
            string secret;
            var userCredential = await _userCredentialRepository.ReadUserCredentialByUUID(userAuthorisationRequest.UUID).FirstOrDefaultAsync();
            var isUserPinProvided = userAuthorisationRequest.UPH != null;
            if (isUserPinProvided)
            {
                // check if user pin is correct
                if (userAuthorisationRequest.UPH == userCredential.UserPIN)
                {
                    secret = userCredential.SharedSecret;
                }
                // This means the uph provided is incorrect
                else
                {
                    return null;
                }
            }
            // user pin is not provided
            else
            {
                // Check if Biometric is enabled
                if (userCredential.BiometricEnabled)
                {
                    secret = userCredential.BiometericSharedSecret;
                }
                else
                {
                    return null;
                }
            }
            _totpService.GenerateTOTP(secret);
            bool isValidated;
            if (isUserPinProvided)
            {
                isValidated = _totpService.ValidateTOTP(userAuthorisationRequest.UPOTP);
            }
            else
            {
                isValidated = _totpService.ValidateTOTP(userAuthorisationRequest.BPOTP);
            }
            if (isValidated)
            {
                return new UserAuthorisationResponse { AccessToken = _tokenGeneratorService.GenerateJSONWebToken() };
            }
            return null;
        }
    }
}