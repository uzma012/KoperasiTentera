using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace KT.Models.Registration.Device.Request
{
    public class PinCodeRequest
    {
        [Required]
        public Guid UUID { get; set; }

        [Required]
        public string UPH { get; set; }
        
    }
}