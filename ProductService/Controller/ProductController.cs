using Microsoft.AspNetCore.Mvc;
using ProductService.Data;
using ProductService.Model.Dtos;
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

            var products = await httpClient.GetFromJsonAsync<List<ProductDto>>(adapterUrl);

            if (products == null || !products.Any())
                return BadRequest("No products returned.");

            var baseProducts = products.Select(p =>new Product
            {
                Provider = p.Provider,
                Name = p.Name, 
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                PruductType = p.PruductType
            }).ToList();

            dbContext.Products.AddRange(baseProducts); //Add multiple entities efficiently
            await dbContext.SaveChangesAsync(); //freeing up the thread while waiting for DB

            foreach (var product in products)
            {
                var insertProduct = baseProducts.First(p=>p.Name == product.Name);

                if (product.PruductType == "novel")
                {
                    dbContext.BookDetails.Add(new BookDetails
                    {
                        ProductId = insertProduct.ProductId,
                        Author = product.Author,
                        Publisher = product.Publisher,
                        Category = product.Category
                    });
                }
            }

            await dbContext.SaveChangesAsync();

            return Ok(new { Imported = products.Count });
        }
    }
}
