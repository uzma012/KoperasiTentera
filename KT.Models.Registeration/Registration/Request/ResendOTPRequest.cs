using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KT.Models.Registration.Registration.Request
{
    public class ResendOTPRequest
    {
        public string UUID { get; set; }
        public string Email { get; set; }

        [Required]
        public string UDID { get; set; }

        public string PhoneNumber { get; set; }
    }
}