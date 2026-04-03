using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
            this._configuration = configuration;
            this._httpContextAccessor = httpContextAccessor;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequeste request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);

            if (user is null)
                return new LoginResponse() { Success = false, Message = "invalid email" };

            if (!await userManager.IsEmailConfirmedAsync(user))
                return new LoginResponse() { Success = false, Message = "email not confirmed" };


            var result = await userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
                return new LoginResponse() { Success = false, Message = "invalid email" };



            return new LoginResponse() { Success = true, Message = "success", AccessToken = await GenerateAccessToken(user) };
        }

        private async Task<string> GenerateAccessToken(ApplicationUser user)
        {
            var userClaims = new List<Claim>() {

                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email)

            };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(5),
                signingCredentials: credentials
        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<RegiseterResponse> RegisterAsync(RegisterRequeste request)
        {
            var user = request.Adapt<ApplicationUser>();

            var result = await userManager.CreateAsync(user, request.Password);



            if (!result.Succeeded)
            {
                return new RegiseterResponse
                {
                    Success = false,
                    Message = "Registration failed",
                    Error= result.Errors.Select(e=>e.Description).ToList()
                };
            }


            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            token = Uri.EscapeDataString(token);


            var emailUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/Account/ConfirmEmail?token={token}&userId={user.Id}";
            await userManager.AddToRoleAsync(user, "User");
            await emailSender.SendEmailAsync(
                     user.Email,
                     "welcome",
                     $"<h2>welcome {request.UserName}</h2>" +
                     $"<a href='{emailUrl}'>Confirm Email</a>"
             );
            return new RegiseterResponse
            {
                Success = true,
                Message = "User registered successfully"
            };

        }
        public async Task<bool> ConfirmEmailAsync(string token, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return false;

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return false;
            return true;
        }

        public async Task<ForgotPasswordResponse> RequestPasswordResetAsync(ForgotPasswordRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ForgotPasswordResponse() { Success = false, Message = "email not found" };
            }

            var random = new Random();
            var code = random.Next(1000,9999).ToString();

            user.CodeResetPassword = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(15);

            await userManager.UpdateAsync(user);
            await emailSender.SendEmailAsync(request.Email,"reset password",$"<p> code is {code}</p>");

            return new ForgotPasswordResponse() { Success = true, Message = "code send to your email" };

        }

        public async Task<ResetPassswordResponse> ResetPassswordAsync(ResetPassswordRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return new ResetPassswordResponse() { Success = false, Message = "email not found" };
            }
            else if (user.CodeResetPassword == null)
            {

                return new ResetPassswordResponse() { Success = false, Message = "invalid code" };
            }
            else if (user.PasswordResetCodeExpiry < DateTime.UtcNow)
            {
                return new ResetPassswordResponse() { Success = false, Message = "expired code" };
            }

            var isSamePassword = await userManager.CheckPasswordAsync(user, request.NewPassword);

            if (isSamePassword)
            {
                return new ResetPassswordResponse() { Success = false, Message = "New Password Must by Different from old password" };


            }

            var Token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user,Token, request.NewPassword);

            if (!result.Succeeded)
            {
                return new ResetPassswordResponse() { Success = false, Message = "password reset failed" };

            }
            await emailSender.SendEmailAsync(request.Email, "change password", "your password is changing");

            return new ResetPassswordResponse()
            {
                Success = true,
                Message = "password reset successful"
            };


        }
    }
}
