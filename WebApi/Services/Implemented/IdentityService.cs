using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using WebApi.Controllers.Responses;
using WebApi.Data;
using WebApi.Domain;
using WebApi.Models;

namespace WebApi.Services.Implemented
{
    public class IdentityService : IIdentityService
    {

        private readonly ApplicatinDbContext _applicationDbContext;
        private readonly AppSettings _appSettings;
        private readonly UserManager<IdentityUser> _userManager;
       
        public IdentityService(ApplicatinDbContext applicationDbContext,
            IOptions<AppSettings> appSettings,
            UserManager<IdentityUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _appSettings = appSettings.Value;
            _userManager = userManager;
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

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if(existingUser != null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User with this email address already exists" }                
                };
            }

            var newUser = new IdentityUser
            {
                Email = email,
                UserName = email
            };

            var createUser = await _userManager.CreateAsync(newUser, password);

            if(!createUser.Succeeded)
            {
                return new AuthenticationResult
                {
                    Errors = createUser.Errors.Select(x => x.Description)
                };
            }
            return GetAuthenticationResultForUser(newUser);
        }
        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User dose not exists" }
                };
            }
            var userValidPassword = await _userManager.CheckPasswordAsync(user, password);

            if (!userValidPassword)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "User password combination is wrong" }
                };
            }

            return GetAuthenticationResultForUser(user);
        }
        private AuthenticationResult GetAuthenticationResultForUser(IdentityUser  newUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keySecret = Encoding.ASCII.GetBytes(_appSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type: JwtRegisteredClaimNames.Sub, value: newUser.Email),
                    new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
                    new Claim(type: JwtRegisteredClaimNames.Email, value: newUser.Email),
                    new Claim(type: "id",value: newUser.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keySecret), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }

       
    }
}
