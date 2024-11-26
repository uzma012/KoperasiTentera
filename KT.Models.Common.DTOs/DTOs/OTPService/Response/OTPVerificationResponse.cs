using System;
using System.Collections.Generic;
using System.Text;

namespace KT.Models.Common.DTOs.OTPService
{
    public class OTPVerificationResponse
    {
        public enum OTPStatus
        {
            Expired,
            Incorrect,
            Correct
        }

        public OTPStatus OTPVerificationStatus { get; set; }
        public int? AttempsRemaining { get; set; }
    }
}