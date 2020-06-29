using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WebApi.Controllers.Responses;
using WebApi.Models;
using WebApi.Models.Requests;
using WebApi.Services;

namespace WebApi.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }
        //[HttpPost(template: ApiRoutes.Identity.Login)]
        //public IActionResult Post([FromBody] User value)
        //{
        //    var user = _identityService.Authenticate(value.UserName, value.Password);
        //    if(user==null)
        //    {
        //        return BadRequest(new { message = "User name or password is incorrect" });
        //    }
        //    return Ok(user);
        //}

        [HttpPost(template:ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest value)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(z => z.ErrorMessage))
                });
            }
            var authResponse = await _identityService.RegisterAsync(value.Email, value.Password);

            if(!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            return Ok( new AuthSuccessResponse 
            { 
                Token =authResponse.Token
            });
        }
        [HttpPost(template: ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest value)
        {
            var authResponse = await _identityService.LoginAsync(value.Email, value.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token
            });
        }
    }
}
