using AdapterService.Services.Interfeces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdapterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAdapterController : ControllerBase
    {
        private readonly IProductAdapter _adapter;

        public ProductAdapterController(IProductAdapter adapter)
        {
            _adapter = adapter;
        }

        [HttpGet]
        public async Task<IActionResult> GetFromAdapter()
        {
            var products = await _adapter.GetProductsAsync();
            return Ok(products);
        }
    }
}
