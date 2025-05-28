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
        public async Task<IActionResult> GetFromAdapter([FromQuery] string provider)
        {
            var adapter = this.adapterFactoryService.Factory(provider);
            var products = await adapter.GetProductsAsync();
            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> PostToAdapter([FromQuery] string provider, [FromBody] CartReqDto dto)
        {
            var adapter = this.adapterFactoryService.Factory(provider);
            var response = await adapter.AddToCartAsync(dto);

            if (!response)
            {
                return BadRequest("Failed to add product to cart.");
            }

            return Ok("Successfully add to cart.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFromAdapter([FromQuery] string provider, [FromBody] ItemRemoveReqDto dto)
        {
            var adapter = this.adapterFactoryService.Factory(provider);
            var response = await adapter.RemoveFromCartAsync(dto);
            if (!response)
            {
                return BadRequest("Failed to remove product from cart.");
            }
            return Ok("Successfully removed from cart.");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateItem([FromQuery] string provider, [FromBody] CartReqDto dto)
        {
            var adapter = this.adapterFactoryService.Factory(provider);
            var response = await adapter.UpdateItemAsync(dto);
            if (!response)
            {
                return BadRequest("Failed to update item in cart.");
            }
            return Ok("Successfully updated item in cart.");
        }
    }
}
