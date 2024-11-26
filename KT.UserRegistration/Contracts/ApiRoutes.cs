namespace PFM.Registration.Contracts
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class Registration
        {
            public const string RegistrationBase = Base + "/registration";
            public const string loginBase = Base + "/login";
            public const string PostRegistrationDetails = RegistrationBase;
            public const string registrationDetails = RegistrationBase + "/enrol";
            public const string VerifyOTP = RegistrationBase + "/" + "otp";

            public const string ResendOTP = VerifyOTP + "/" + "resend";

        }

        public static class User
        {
            public const string UserBase = Base + "/user";

            public static class Device
            {

                public const string DeviceBase = UserBase + "/device";

                public const string Pin = DeviceBase + "/pin";
                public const string Biometric = DeviceBase + "/biometric";
                public const string DisableBiometric = Biometric + "/disable";
                public const string PinReset = Pin + "/reset";
                public const string Enrol = DeviceBase + "/enrol";
            }

            public static class Authorise
            {
                public const string AuthoriseBase = UserBase + "/authorise";
            }


        
        }
    }
}