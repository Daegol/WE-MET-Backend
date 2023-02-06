using Caching.Interfaces;
using Data.Mongo.Collections;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Account;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILoginLogService _loginLogService;
        private readonly ICacheManager _cache;
        public AccountController(IAccountService accountService, ILoginLogService loginLogService, ICacheManager cache)
        {
            _accountService = accountService;
            _loginLogService = loginLogService;
            _cache = cache;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            var result = await _accountService.AuthenticateAsync(request);
            if (result.Errors == null || !result.Errors.Any())
            {
                //LoginLog log = new LoginLog()
                //{
                //    LoginTime = DateTime.Now,
                //    UserName = request.UserName
                //};
                //await _loginLogService.Add(log);
            }
            return Ok(result);
        }
        [Authorize(Policy = "OnlyAdmins")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            _cache.Clear();
            var uri = $"{Request.Scheme}://{Request.Host.Value}";
            return Ok(await _accountService.RegisterAsync(request, uri));
        }

        //niepotrzebne na razie
        /*[HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            return Ok(await _accountService.ConfirmEmailAsync(userId, code));
        }*/

        //na razie chyba niepotrzebne bo użytkownik sam nie może nic zmieniać
        /*
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var uri = $"{Request.Scheme}://{Request.Host.Value}";
            await _accountService.ForgotPasswordAsync(request, uri);
            return Ok();
        }*/
        [Authorize(Policy = "OnlyAdmins")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return Ok(await _accountService.ResetPasswordAsync(request));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
        {
            return Ok(await _accountService.ChangePasswordAsync(request));
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest request)
        {
            return Ok(await _accountService.RefreshTokenAsync(request));
        }

        [HttpGet("logout")]
        public async Task<IActionResult> LogoutAsync(string userName)
        {
            return Ok(await _accountService.LogoutAsync(userName));
        }

        private string GenerateIPAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
