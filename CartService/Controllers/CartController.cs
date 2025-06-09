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
        public async Task<IActionResult> AddToCart([FromBody] CartItemReqDto dto)
        {
            bool isReseved = await cartService.AddItemToCart(dto);

            if (!isReseved)
            {
                return BadRequest("Insufficient stock for the product.");
            }

            return Ok("Item added to cart and event published.");
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFromCart([FromQuery] Guid CartItemID)
        {
            await cartService.RemoveItemFromCart(CartItemID);
            return Ok("Item removed from cart and event published.");
        }

        [HttpPatch("update")]
        public async Task<IActionResult> UpdateCartItem([FromBody] CartItemUpdateReqDto dto)
        {
            await cartService.UpdateCartItem(dto);
            return Ok("Cart item updated and event published.");
        }

        [HttpGet("getItemsByUserId")]
        public async Task<IActionResult> GetCartItems([FromQuery] Guid userId)
        {
            var items = await cartService.GetCartItems(userId);
            return Ok(items);
        }
    }
}
