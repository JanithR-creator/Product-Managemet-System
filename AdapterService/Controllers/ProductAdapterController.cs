using AdapterService.Models.Dtos.InternalDtos;
using AdapterService.Services.FactoryService;
using Microsoft.AspNetCore.Mvc;

namespace AdapterService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAdapterController : ControllerBase
    {
        private readonly IProductFactoryService adapterFactoryService;

        public ProductAdapterController(IProductFactoryService adapterFactoryService)
        {
            this.adapterFactoryService = adapterFactoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFromAdapter()
        {
            var adapters = this.adapterFactoryService.GetAllAdapters();
            List<Product> allProducts = new List<Product>();

            foreach (var adapter in adapters)
            {
                var products = await adapter.GetProductsAsync();
                allProducts.AddRange(products);
            }

            return Ok(allProducts);
        }

        [HttpPost("make-payment")]
        public async Task<IActionResult> MakePayment([FromQuery] string provider, [FromBody] PaymentReqDto dto)
        {
            var adapter = this.adapterFactoryService.Factory(provider);
            var response = await adapter.MakePaymentAsync(dto);
            if (!response)
            {
                return BadRequest("Failed to make payment.");
            }
            return Ok("Payment successful.");
        }
    }
}
