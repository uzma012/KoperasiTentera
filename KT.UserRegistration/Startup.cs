
using KT.Common;
using KT.Interfaces.Repositories;
using KT.Interfaces.Services.Common;
using KT.Models.Common.Options;
using KT.Models.DBContext;
using KT.Registration.Services.OTP;
using KT.Registration.Services.Registration;
using KT.Registration.Services.User;
using KT.Repositories;
using KT.Validators.Registration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace UserRegistration
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Registration", Version = "v1" });
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
            });
            var deployedDbString = Configuration.GetConnectionString("dbConnection");
            services.AddDbContext<ApplicationContext>(options =>
            options.UseSqlServer(deployedDbString));
            services.AddTransient<RegistrationService>();
            services.AddTransient<UserDeviceService>();
            services.AddTransient<TokenGeneratorService>();
            services.AddTransient<SharedSecretGeneratorService>();
            services.AddTransient<IOTPService, OTPService>();
            services.AddTransient<ITOTPService, TOTPService>();
            services.AddTransient<ITokenGeneratorService, TokenGeneratorService>();
            services.AddTransient<ISharedSecretGeneratorService, SharedSecretGeneratorService>();
            services.AddTransient<TOTPService>();
            services.AddTransient<OTPService>();
            services.AddTransient<PhoneNumberValidator>();
            services.AddTransient<UserOTPRepository>();
            services.AddTransient<UserCredentialRepository>();
            services.AddTransient<AccountRepository>();

            services.Configure<OTPOptions>(Configuration.GetSection(nameof(OTPOptions)));
            services.Configure<JWTTokenOptions>(Configuration.GetSection(nameof(JWTTokenOptions)));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IUserCredentialRepository), typeof(UserCredentialRepository));
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Registration v1"));
            }
            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
            });
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}