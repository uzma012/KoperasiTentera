
using KT.Models.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Twilio.Rest.Lookups.V1;

namespace KT.Validators.Registration
{
    public class PhoneNumberValidator
    {
        public readonly IConfiguration _configuration;

        public PhoneNumberValidator()
        {
        }

        public PhoneNumberValidator(IOptions<OTPOptions> otpOptions, IConfiguration configuration)
        {
        }
        public bool ValidatePhoneNumber(string phoneNumber)
        {
            try
            {
                if (phoneNumber.Length != 14 && phoneNumber.Length != 13)
                {
                    return false;
                }
                var plusSign = phoneNumber.Substring(0, 0);
                var countryCode = phoneNumber.Substring(1, 2);
                if (countryCode == "60")
                {
                    if (phoneNumber.Length == 13)
                    {
                        var mobileNumber = phoneNumber.Substring(4, 8);
                    }
                    else
                    {
                        var mobileNumber = phoneNumber.Substring(3, 8);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    
    }
}