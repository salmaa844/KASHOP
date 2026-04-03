using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var result = await _brandService.GetAllBrandAsync();
            if (result == null) return BadRequest();
            return Ok(result);
        }
        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> CreateBrand([FromForm] BrandRequest response)
        {
            await _brandService.CreateBrandAsync(response);
            return Ok();

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var result = await _brandService.GetBrandAsync(b => b.Id == id);
            if (result == null) return BadRequest();
            return Ok(new { data = result });
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteBrand(int id)
        {
            var deleted = await _brandService.DeleteAsync(id);
            if(!deleted ) return BadRequest();
            return Ok();
        } 
    }
}
