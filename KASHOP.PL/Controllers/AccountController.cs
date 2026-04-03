using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            this._authenticationService = authenticationService;
        }
        [HttpPost("register")]

        public async Task<IActionResult> Register(RegisterRequeste register)
        {
            var result = await _authenticationService.RegisterAsync(register);
            return Ok(result);

        }

        [HttpPost("login")]

        public async Task<IActionResult> Login(LoginRequeste request)
        {
            var result = await _authenticationService.LoginAsync(request);
            return Ok(result);
        }
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var result = await _authenticationService.ConfirmEmailAsync(token, userId);

            if (!result)
                return BadRequest("Invalid token");

            return Ok("Email confirmed");
        }

        [HttpPost("SendCode")]
        public async Task<IActionResult> RequestPasswordReset(ForgotPasswordRequest request)
        {
            var result = await _authenticationService.RequestPasswordResetAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassswordRequest request)
        {
            var result = await _authenticationService.ResetPassswordAsync(request);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
