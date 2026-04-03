using KASHOP.BLL.Service;
using KASHOP.DAL.DTO.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProducts();
            return Ok(new
            {
                data = products
            });
        }
        [HttpPost("")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] ProductRequest request)
        {
            await _productService.CreateProduct(request);
            return Ok();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Index(int id)
        {
            var products = await _productService.GetProduct(P => P.Id == id);
            if(products == null) return NotFound();

            return Ok(new
            {
                data = products
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteProduct(id);
            if (!deleted) return BadRequest();

            return Ok();
        }
        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id ,[FromForm] ProductUpdateRequest request)
        {
            var updated = await _productService.UpdateProductAsync(id, request);
            if(!updated) return BadRequest();
            return Ok();
        }
    }
}
