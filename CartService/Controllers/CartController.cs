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

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateCartItem([FromQuery] string provider, [FromBody] CartItemUpdateReqDto dto)
        {
            await cartService.UpdateCartItem(dto, provider);
            return Ok("Cart item updated and event published.");
        }

        [HttpGet("getItemsByUserId")]
        public async Task<IActionResult> GetCartItems([FromQuery] Guid userId)
        {
            var items = await cartService.GetCartItems(userId);
            return Ok(items);
        }

        [HttpGet("getAll")]
        public async Task<IActionResult> GetAllCartItems()
        {
            var items = await cartService.GetAllCartItems();
            return Ok(items);
        }
    }
}
