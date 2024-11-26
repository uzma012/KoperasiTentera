using KT.Interfaces.Services.Common;
using OtpNet;
using System.Security.Cryptography;
using System.Text;

namespace KT.Common
{
    public class TOTPService : ITOTPService
    {
        private Totp _totpGenerator;
        private int TOTPSize = 8;
        private int TOTPStep = 30;
        private OtpHashMode OTPHashMode = OtpHashMode.Sha256;
        private long TimesUsed = 0;
        private VerificationWindow _window = new VerificationWindow(previous: 1, future: 1);

        private byte[] StringToBase32Encoded(string secret)
        {
            var secretInBytes = Encoding.ASCII.GetBytes(secret);
            var secretInBase32String = Base32Encoding.ToString(secretInBytes);
            return Base32Encoding.ToBytes(secretInBase32String);
        }

        public string GenerateTOTP(string secret)
        {
            var encodedSecret = StringToBase32Encoded(secret);
            _totpGenerator = new Totp(encodedSecret, mode: OTPHashMode, step: TOTPStep, totpSize: TOTPSize);
            var now = DateTime.Now.ToUniversalTime();
            return _totpGenerator.ComputeTotp(now);
        }

        public bool ValidateTOTP(string incomingOTP)
        {
            return _totpGenerator.VerifyTotp(incomingOTP, out TimesUsed, VerificationWindow.RfcSpecifiedNetworkDelay);
        }

    }
}
