using CartService.Models.Dtos.RequestDtos;
using CartService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartService;

        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromQuery] string provider, [FromBody] CartItemReqDto dto)
        {
            await cartService.AddItemToCart(dto, provider);
            return Ok("Item added to cart and event published.");
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart([FromQuery] string provider, [FromBody] Guid CartItemID)
        {
            await cartService.RemoveItemFromCart(CartItemID, provider);
            return Ok("Item removed from cart and event published.");
        }
    }
}
