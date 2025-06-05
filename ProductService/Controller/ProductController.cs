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

        [HttpPost("internal-create")]
        public IActionResult CreateInternalProduct([FromForm] InternalProductReqDto dto)
        {
            productService.CreateInternalProduct(dto);
            return Ok("Product Created Successfully.");
        }

        [HttpPut("internal-update")]
        public IActionResult UpdateProductInternalAsync([FromForm] InternalProductReqDto dto,[FromQuery] Guid productId)
        {
            productService.UpdateProductInternalAsync(dto, productId);
            return Ok("Product Updated Successfully.");
        }

        [HttpDelete("internal-delete")]
        public IActionResult DeleteInternalProductAsync([FromQuery] Guid productId)
        {
            productService.DeleteInternalProductAsync(productId);
            return Ok("Product Deleted Successfully.");
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportFromAdapter([FromQuery] string provider)
        {


            var importedCount = await productService.SaveProducts(provider);

            if (importedCount == 0)
                return BadRequest("No products returned from adapter.");

            return Ok(new { Imported = importedCount });
        }

        [HttpGet("novel/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await productService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsAsync([FromQuery] string productType, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? filter = null)
        {
           var data =await productService.GetProductsAsync(productType, page, pageSize, filter);

            return Ok(data);
        }

        [HttpGet("providers")]
        public async Task<IActionResult> GetAllProvidersAsync()
        {
            var providers = await productService.GetAllProvidersAsync();
            return Ok(providers);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllProductsAsync([FromQuery] string? provider = null, [FromQuery] string? filter = null)
        {
            var products = await productService.GetAllProductsAsync(provider, filter);
            return Ok(products);
        }
    }
}
