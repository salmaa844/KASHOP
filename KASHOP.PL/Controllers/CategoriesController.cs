using KASHOP.BLL.Service;
using KASHOP.DAL.Data;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using KASHOP.PL.Resources;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IStringLocalizer<SharedResources> localizer;

        public CategoriesController(ICategoryService _categoryService , IStringLocalizer<SharedResources> localizer)
        {
            this._categoryService = _categoryService;
            this.localizer = localizer;
        }
        [HttpPost("")]
        public async Task<IActionResult> Create(CategoryRequeste requeste)
        {
            
           var res = await _categoryService.CreateCategoryAsync(requeste);

            return Ok(new
            {
                massege=localizer["Success"].Value,
                 res
            });
        }
        [HttpGet("")]
        public async Task<IActionResult> GetCategories()
        {
           var category = await _categoryService.GetAllCategoriesAsync();
           
            return Ok(new{
                data = category,
                localizer["Success"].Value });

        }
    }
}
