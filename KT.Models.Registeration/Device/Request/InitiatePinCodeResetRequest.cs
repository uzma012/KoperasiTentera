using System.ComponentModel.DataAnnotations;

namespace KT.Models.Registration.Device.Request
{
    public class InitiatePinCodeResetRequest
    {
        [Required]
        public string UDID { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}