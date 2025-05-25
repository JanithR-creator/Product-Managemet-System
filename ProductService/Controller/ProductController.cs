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
    }
}
