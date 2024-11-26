using KT.Models.Registration.Device.Request;
using System.ComponentModel.DataAnnotations;


namespace KT.Models.Registration.Registration.Request
{
    public class LoginRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string ICNumber { get; set; }
        public Guid UDID { get; set; }
    }
}
