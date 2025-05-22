using AdapterService.Services.FactoryService;
using Microsoft.AspNetCore.Mvc;

namespace AdapterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAdapterController : ControllerBase
    {
        private readonly IProductAdapterFactoryService adapterFactoryService;

        public ProductAdapterController(IProductAdapterFactoryService adapterFactoryService)
        {
            this.adapterFactoryService = adapterFactoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFromAdapter([FromQuery] string provider)
        {
            var adapter = this.adapterFactoryService.Factory(provider);
            var products = await adapter.GetProductsAsync();
            return Ok(products);
        }
    }
}
