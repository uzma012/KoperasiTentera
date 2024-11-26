
using KT.Models.DB.User;

namespace KT.Interfaces.Services.Common
{
    public interface ITokenGeneratorService
    {
        public string GenerateJSONWebToken();

        public string GenerateJwtToken(string uuid, AccountModel accountModel);
    }
}