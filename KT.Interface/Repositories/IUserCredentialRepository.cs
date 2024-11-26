using KT.Models.DB.User;
using KT.Models.Registration.Registration.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KT.Interfaces.Repositories
{
    public interface IUserCredentialRepository
    {
        public Task<string> CreateOrUpdateUserCredential(OTPRequest otp, UserCredentialModel userCredentialModel);

        public Task<UserCredentialModel> UpdateUserPin(Guid uuid, UserCredentialModel userCredentialModel);

        public Task<UserCredentialModel> UpdateUserBiometric(Guid uuid, UserCredentialModel userCredentialModel);

        public IQueryable<UserCredentialModel> ReadUserCredentialByUUID(Guid uuid);

        public Task<UserCredentialModel> RemoveUserPinAndGetUserDetails( Guid uuid);

        public void UpdateUserCredential(UserCredentialModel userCredentialModel);
    }
}