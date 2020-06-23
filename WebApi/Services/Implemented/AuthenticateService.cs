using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services.Implemented
{
    public class AuthenticateService : IAuthenticateService
    {

        private readonly ApplicatinDbContext _applicationDbContext;
        private readonly AppSettings _appSettings;
        public AuthenticateService(ApplicatinDbContext applicationDbContext, IOptions<AppSettings> appSettings)
        {
            _applicationDbContext = applicationDbContext;
            _appSettings = appSettings.Value;
        }
        public User Authenticate(string userName, string password)
        {
            // var user = _applicationDbContext.Users.FirstOrDefault(u=>u.UserName==userName && u.PasswordHash == password);
            var user = new User
            {
                UserName = userName,
                Password = password
            };
            if (user == null)
                return null;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role,"Admin"),
                    new Claim(ClaimTypes.Version, "V3.1")
                }),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            user.Token = tokenHandler.WriteToken(token);

            user.Password = null;

            return user;
        }
    }
}
