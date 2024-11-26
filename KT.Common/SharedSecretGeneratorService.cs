
using KT.Interfaces.Services.Common;
using System.Security.Cryptography;

namespace KT.Common
{
    public class SharedSecretGeneratorService : ISharedSecretGeneratorService
    {
        public string GetPad()
        {
            using (Aes aes = Aes.Create())
            {
                return Convert.ToBase64String(aes.Key);
            }
        }
    }
}
