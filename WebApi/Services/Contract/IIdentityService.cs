using Microsoft.AspNetCore.Authentication;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApi.Domain;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IIdentityService
    {
        User Authenticate(string userName, string password);
        Task<AuthenticationResult> RegisterAsync(string email, string password);
    }
}
