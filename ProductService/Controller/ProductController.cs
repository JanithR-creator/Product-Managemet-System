using Microsoft.AspNetCore.Mvc;
using ProductService.Model.Dtos.RequestDtos;
using ProductService.Services;

namespace ProductService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpDelete]
        public IActionResult DeleteAllProducts() 
        {
            productService.DeleteAllProducts();
            return Ok("All Products Successfully Deleted.");        
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportFromAdapter([FromQuery] string provider)
        {


            var importedCount = await productService.SaveProducts(provider);

            if (importedCount == 0)
                return BadRequest("No products returned from adapter.");

            return Ok(new { Imported = importedCount });
        }

        [HttpGet("novel")]
        public async Task<IActionResult> GetAllNovels([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
           var data =await productService.GetAllNovels(page, pageSize);

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                Items = data
            });
        }

        [HttpGet("schoolItems")]
        public async Task<IActionResult> GetAllSclItems([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var data = await productService.GetAllSclItems(page, pageSize);

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                Items = data
            });
        }
    }
}
