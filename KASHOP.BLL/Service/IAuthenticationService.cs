using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IAuthenticationService
    {
        Task<RegiseterResponse> RegisterAsync(RegisterRequeste register);
        Task<LoginResponse> LoginAsync(LoginRequeste request);
        Task<bool> ConfirmEmailAsync(string token, string id);
        Task<ForgotPasswordResponse> RequestPasswordResetAsync(ForgotPasswordRequest request);
        Task<ResetPassswordResponse> ResetPassswordAsync(ResetPassswordRequest request);
    }
}
