using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IFileService
    {
        Task<string?> UploadeAsync(IFormFile file);
        string GetImageUrl(string fileName);
        void Delete (string  fileName);
    }
}
