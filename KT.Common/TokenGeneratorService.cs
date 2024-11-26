

using KT.Interfaces.Services.Common;
using KT.Models.Common.Options;
using KT.Models.DB.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KT.Common
{
    public class TokenGeneratorService : ITokenGeneratorService
    {
        private readonly IOptions<JWTTokenOptions> _options;
        private readonly IConfiguration _configuration;

        public TokenGeneratorService(IOptions<JWTTokenOptions> options, IConfiguration configuration)
        {
            _options = options;
            _configuration = configuration;
        }

        public string GenerateJSONWebToken()
        {
            //var key = _configuration["JWTTokenOptions:Key"];
            var key = _options.Value.Key;
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_options.Value.Issuer,
              _options.Value.Audience,
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public string GenerateJwtToken(string uuid, AccountModel account)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string accountId = account.AccountId.ToString();
            string accountName = account.AccountName;
            var key = Encoding.UTF8.GetBytes(_configuration["JWTKey:Key"]);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("uuid", uuid));
            claims.Add(new Claim("accounts", accountId));
            claims.Add(new Claim("accountName", accountName));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = _configuration["JWTTokenOptions:Issuer"],
                Audience = _configuration["JWTTokenOptions:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }



      
        public class ClaimRecord
        {
            public string uuid { get; set; }

            public string udid { get; set; }

            public List<long> accounts { get; set; }
        }
    }
}