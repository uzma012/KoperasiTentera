using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KT.Models.Registration.Device.Request
{
    public class BiometricRequest
    {
        [Required]
        public Guid UUID { get; set; }
        [Required]
        public Guid UDID { get; set; }
    }
}
