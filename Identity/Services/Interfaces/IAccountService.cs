using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Models;
using Models.DTOs.Account;
using Models.ResponseModels;

namespace Identity.Services.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request);
        Task<BaseResponse<string>> RegisterAsync(RegisterRequest request, string uri);
        Task<BaseResponse<string>> ConfirmEmailAsync(string userId, string code);
        Task ForgotPasswordAsync(ForgotPasswordRequest request, string uri);
        Task<BaseResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<BaseResponse<string>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<BaseResponse<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<BaseResponse<string>> LogoutAsync(string userEmail);
        Task<List<ApplicationUser>> GetUsers();
        Task<ApplicationUser> GetOneUser(int id);
        Task<ApplicationUser> GetOneUserWithRoles(int id);
        Task<BaseResponse<string>> DeleteUser(int id);
        Task<BaseResponse<string>> UpdateUser(UpdateRequest request);
    }
}
