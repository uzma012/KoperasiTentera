using System;
using System.Collections.Generic;
using System.Text;

namespace KT.Models.Common.OTP
{
    public enum OTPStateTypeEnum
    {
        PendingVerification = 1,
        Verified = 2,
        Expired = 3
    }
}
