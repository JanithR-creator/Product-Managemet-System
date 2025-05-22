using Microsoft.AspNetCore.Mvc;
using ProductService.Data;
using ProductService.Model.Entity;

namespace ProductService.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public ProductController(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(dbContext.Products.ToList());
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportFromAdapter([FromQuery] string provider = "abc")
        {
            using var httpClient = new HttpClient();
            var adapterUrl = $"http://adapterservice:80/api/ProductAdapter?provider={provider}"; // service name in Docker

            var products = await httpClient.GetFromJsonAsync<List<Product>>(adapterUrl);

            if (products == null) return BadRequest("No products returned from adapter.");

            dbContext.Products.AddRange(products); //Add multiple entities efficiently
            await dbContext.SaveChangesAsync(); //freeing up the thread while waiting for DB

            return Ok(products);
        }
    }
}
