using CheckoutService.Models.Dto.ReqDtos;
using CheckoutService.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            this.checkoutService = checkoutService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCheckout(CheckoutReqDto dto)
        {
            if (dto == null || dto.UserId == Guid.Empty)
                return BadRequest("Invalid checkout request.");

            var result = await checkoutService.CreateCheckoutAsync(dto);

            return Ok(result);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> MakePayment(string provider, [FromBody] PaymentReqDto dto)
        {
            var result = await checkoutService.MakePaymentAsync(provider, dto);

            if (!result)
                return NotFound("Checkout not found or Payment already completed.");

            return Ok("Payment completed.");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCheckout(Guid userId)
        {
           var checkout =  await checkoutService.GetCheckOutByUserIdAsync(userId);

            return Ok(checkout);
        }
    }
}
