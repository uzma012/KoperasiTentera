namespace PFM.Registration.Contracts
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class ProductAds
        {
            public const string products = Base + "/products";
            public const string ads = Base + "/ads";

        }

     
    }
}