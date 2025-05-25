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
            await cartService.AddItemToCart(dto);
            return Ok("Item added to cart and event published.");
        }
    }
}
