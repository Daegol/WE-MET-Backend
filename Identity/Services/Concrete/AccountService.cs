using Core.Exceptions;
using Core.Helpers;
using Core.Interfaces;
using Identity.Models;
using Identity.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Account;
using Models.DTOs.Email;
using Models.Enums;
using Models.ResponseModels;
using Models.Settings;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services.Concrete
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        public AccountService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<JWTSettings> jwtSettings,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(request.UserName.Trim());

            if(user == null)
            {
                throw new ApiException($"Nie znaleziono użytkownika '{request.UserName}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Konto dla użytkownika '{request.UserName}' nie zostało aktywowane. Skontaktuj się z administratorem.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, lockoutOnFailure: false);

            if (!signInResult.Succeeded)
            {
                throw new ApiException($"Niepoprawne dane logowania.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            string ipAddress = IpHelper.GetIpAddress();
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id.ToString();
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.UserName = user.UserName;
            response.FirstName = user.FirstName;
            response.LastName = user.LastName;
            IList<string> rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            response.RefreshToken = await GenerateRefreshToken(user);
            return new BaseResponse<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        public async Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return new BaseResponse<string>(user.Id.ToString(), message: $"Account Confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.");
            }
            else
            {
                throw new ApiException($"An error occured while confirming {user.Email}.") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequest request, string uri)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return;

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var route = "api/account/reset-password/";
            var enpointUri = new Uri(string.Concat($"{uri}/", route));
            var emailRequest = new EmailRequest()
            {
                Body = $"You have to send a request to the '{enpointUri}' service with reset token - {code}",
                To = request.UserName,
                Subject = "Reset Password",
            };
            //TODO forgot password without sending email
        }

        public async Task<BaseResponse<string>> LogoutAsync(string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            }
            await _signInManager.SignOutAsync();

            return new BaseResponse<string>(userName, message: $"Wylogowano.");
        }

        public async Task<BaseResponse<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                throw new ApiException($"Nie znaleziono użytkownika '{request.UserName}'.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Konto dla użytkownika '{request.UserName}' nie zostało aktywowane.Skontaktuj się z administratorem.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            string refreshToken = await _userManager.GetAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            bool isValid = await _userManager.VerifyUserTokenAsync(user, "MyApp", "RefreshToken", request.Token);
            if (!refreshToken.Equals(request.Token) || !isValid)
            {
                throw new ApiException($"Token jest niepoprawny.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }

            string ipAddress = IpHelper.GetIpAddress();
            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, ipAddress);
            AuthenticationResponse response = new AuthenticationResponse();
            response.Id = user.Id.ToString();
            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;
            IList<string> rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            response.RefreshToken = await GenerateRefreshToken(user);

            await _signInManager.SignInAsync(user, false);
            return new BaseResponse<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        public async Task<BaseResponse<string>> RegisterAsync(RegisterRequest request, string uri)
        {
            ApplicationUser findUser = await _userManager.FindByNameAsync(request.UserName);
            if (findUser != null)
            {
                throw new ApiException($"Nazwa użytkownika '{request.UserName}' jest już zajęta. Spróbuj innej.") { StatusCode = (int)HttpStatusCode.BadRequest };
            }
            ApplicationUser newUser = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName
            };
            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (result.Succeeded)
            {
                if (request.UserRoles != null)
                {
                    foreach (var role in request.UserRoles)
                    {
                        await _userManager.AddToRoleAsync(newUser, role.ToString());
                    }
                }

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                await _userManager.ConfirmEmailAsync(newUser, code);

                return new BaseResponse<string>(newUser.Id.ToString(), message: $"Użytkownik został zarejestrowany");
            }
            else
            {
                var errors = "";
                if(result.Errors.ElementAt(0).Code.Contains("Password"))
                {
                    errors = "Hasło musi zawierać przynajmniej 6 znaków, co najamniej jedną małą i dużą literę oraz co najmniej jedną cyfrę oraz znak specjalny";
                    throw new ApiException($"{errors}") { StatusCode = (int)HttpStatusCode.BadRequest };
                }

                throw new ApiException($"{errors}") { StatusCode = (int)HttpStatusCode.InternalServerError };
                
            }
        }

        public async Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) throw new ApiException($"Nie znaleziono konta '{request.UserName}'.");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, code, request.Password);
            if (result.Succeeded)
            {
                return new BaseResponse<string>(request.UserName, message: $"Pomyślnie zresetowano hasło.");
            }
            else
            {
                throw new ApiException($"Wystąpił błąd podczas resetowania hasła. Spróbuj ponownie.");
            }
        }

        public async Task<BaseResponse<string>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) throw new ApiException($"Nie znaleziono konta '{request.UserName}'.");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.Password);
            if (result.Succeeded)
            {
                return new BaseResponse<string>(request.UserName, message: $"Pomyślnie zmieniono hasło.");
            }
            else
            {
                throw new ApiException($"Wystąpił błąd podczas zmiany hasła. Spróbuj ponownie.");
            }
        }

        public async Task<BaseResponse<string>> DeleteUser(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) throw new ApiException($"Niepoprawne dane");

            var result = await _userManager.DeleteAsync(user);
            if(result.Succeeded)
            {
                return new BaseResponse<string>(user.UserName, message: "Pomyślnie usunięto użytkownika.");
            }
            else
            {
                throw new ApiException($"Wystąpił błąd podczas usuwania użytkownika. Spróbuj ponownie.");
            }
        }

        public async Task<List<ApplicationUser>> GetUsers()
        {
            return await _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToListAsync();
        }

        public async Task<ApplicationUser> GetOneUser(int id)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<ApplicationUser> GetOneUserWithRoles(int id)
        {
            return await _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Id == id);
        }

        private async Task<string> GenerateRefreshToken(ApplicationUser user)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "MyApp", "RefreshToken");
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(user, "MyApp", "RefreshToken");
            IdentityResult result = await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", newRefreshToken);
            if (!result.Succeeded)
            {
                throw new ApiException($"An error occured while set refreshtoken.") { StatusCode = (int)HttpStatusCode.InternalServerError };
            }
            return newRefreshToken;
        }

        private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user, string ipAddress)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id.ToString()),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        private async Task<string> SendVerificationEmail(ApplicationUser newUser, string uri)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/account/confirm-email/";
            var _enpointUri = new Uri(string.Concat($"{uri}/", route));
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", newUser.Id.ToString());
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);

            return verificationUri;
        }

        public async Task<BaseResponse<string>> UpdateUser(UpdateRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) throw new ApiException($"Użytkownik '{request.UserName}' nie istnieje");

            user.FirstName = request.FirstName != null ? request.FirstName : user.FirstName;
            user.LastName = request.LastName != null ? request.LastName : user.LastName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                if(request.UserRoles != null)
                {
                    await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
                    foreach (var role in request.UserRoles)
                    {
                        await _userManager.AddToRoleAsync(user, role.ToString());
                    }
                }
                return new BaseResponse<string>(request.UserName, message: "Pomyślnie zaaktualizowano użytkownika.");
            }
            else
            {
                throw new ApiException($"Wystąpił błąd podczas aktualizacji użytkownika. Spróbuj ponownie.");
            }
        }
    }
}
