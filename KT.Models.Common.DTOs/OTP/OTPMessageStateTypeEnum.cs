namespace KT.Models.Common.OTP
{
    public enum OTPMessageStateTypeEnum
    {
        Accepted = 1,
        Queued = 2,
        Sending = 3,
        Sent = 4,
        Delivered = 5,
        Undelivered = 6,
        Failed = 7
    }
}