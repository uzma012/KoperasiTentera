namespace KT.Interfaces.Services.Common
{
    public interface ITOTPService
    {
        public string GenerateTOTP(string secret);

        public bool ValidateTOTP(string incomingOTP);
    }
}