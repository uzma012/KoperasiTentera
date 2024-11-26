using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KT.Models.Registration.Registration.Request
{
    public class OTPRequest
    {
        public string Email { get; set; } = null;
        public string PhoneNumber { get; set; } = null;
        public string UUID { get; set; } = null;
        [Required]
        public string Otp { get; set; }
    }
}
