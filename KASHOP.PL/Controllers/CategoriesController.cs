using KASHOP.BLL.Service;
using KASHOP.DAL.Data;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repositry;
using KASHOP.PL.Resources;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
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
        [Authorize]
        public async Task<IActionResult> Create(CategoryRequeste requeste)
        {
           var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
           var res = await _categoryService.CreateCategoryAsync(requeste);

            return Ok(new
            {
                massege=localizer["Success"].Value,
                 res
            });
        }
        
        [HttpGet("")]
        public async Task<IActionResult> GetAllCategories()
        {
            var category = await _categoryService.GetAllCategoriesAsync();

            return Ok(new
            {
                data = category,
                localizer["Success"].Value
            });

        }
       
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoriesById(int id)
        {

            return Ok(await _categoryService.GetCategoriesAsync(c => c.Id == id));

        }
       
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UpdateCategoryTranslationRequest request)
        {
            var updatedTranslation = await _categoryService.UpdateCategory(id,request);

            if (updatedTranslation == null)
                return NotFound("Translation with this language does not exist.");

            return Ok(updatedTranslation);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteCategory(id);
            if (!deleted)
            {
                return NotFound(new
                {
                    massege = localizer["Not Found"].Value

                });
            }
            return Ok(new
            {
                massege = localizer["Success"].Value,

            });
        }

    }
}
