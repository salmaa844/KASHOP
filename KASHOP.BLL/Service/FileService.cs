using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{

public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly string _imageFolderPath;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;

            // مجلد الصور داخل wwwroot/images
            _imageFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

            // إنشاء المجلد إذا لم يكن موجودًا
            if (!Directory.Exists(_imageFolderPath))
            {
                Directory.CreateDirectory(_imageFolderPath);
            }
        }

        // رفع الصورة وحفظها باسم عشوائي
        public async Task<string?> UploadeAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_imageFolderPath, fileName);

            using (var stream = File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return fileName; // نخزن فقط اسم الصورة في قاعدة البيانات
        }

        // توليد رابط الصورة ديناميكيًا من BaseUrl
        public string GetImageUrl(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return null;

            var baseUrl = _configuration["AppSettings:BaseUrl"];
            return $"{baseUrl}/images/{fileName}";
        }

        public void Delete(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            if(File.Exists(path)) File.Delete(path);
        }
    }
}

