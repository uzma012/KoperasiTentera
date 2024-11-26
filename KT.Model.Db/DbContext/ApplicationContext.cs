using KT.Models.DB.Ads;
using KT.Models.DB.OTP;
using KT.Models.DB.Products;
using KT.Models.DB.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;


namespace KT.Models.DBContext
{
    public class ApplicationContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;

        public ApplicationContext(DbContextOptions<ApplicationContext> options,
            IConfiguration configuration,
            ILoggerFactory loggerFactory) : base(options)
        {
            try
            {
                //SqlConnection sqlConnection = new SqlConnection();
                _loggerFactory = loggerFactory;
                _configuration = configuration;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductModel>()
                  .Property(p => p.CreatedAt)
                  .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<ProductModel>()
                .Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<AdsModel>()
                .Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<AdsModel>()
                .Property(a => a.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_configuration["ConnectionStrings:dbConnection"]);
            optionsBuilder.UseLoggerFactory(_loggerFactory); // Optional: to use logging
            optionsBuilder.EnableSensitiveDataLogging();
        }
        public DbSet<ApplicationUserModel> ApplicationUser { get; set; }
        public DbSet<UserOTPModel> UserOTP { get; set; }
        public DbSet<UserOTPMessageModel> UserOTPMessage { get; set; }
        public DbSet<UserDeviceModel> UserDevice { get; set; }
        public DbSet<UserCredentialModel> UserCredential { get; set; }
        public DbSet<AccountModel> Account { get; set; }
        public DbSet<UserStateTypeModel> UserStateType { get; set; }
        public DbSet<OTPChannelTypeModel> TOTPChannelType { get; set; }
        public DbSet<OTPMessageStateTypeModel> OTPMessageStateType { get; set; }
        public DbSet<OTPPurposeTypeModel>  OTPPurposeType { get; set; }
        public DbSet<OTPStateTypeModel> OTPStateType { get; set; }
        public DbSet<AdsModel> Ads { get; set; }
        public DbSet<ProductModel> Products { get; set; }
    }
}