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
        public async Task<IActionResult> MakePayment([FromBody] PaymentReqDto dto)
        {
            var result = await checkoutService.MakePaymentAsync(dto);

            if (!result)
                return NotFound("Checkout not found or Payment already completed.");

            return Ok("Payment completed.");
        }

        [HttpGet("{checkoutId}")]
        public async Task<IActionResult> GetCheckoutById(Guid checkoutId)
        {
            if (checkoutId == Guid.Empty)
                return BadRequest("Invalid checkout ID.");
            var result = await checkoutService.GetCheckOutByCheckoutIdAsync(checkoutId);
            if (result == null)
                return NotFound("Checkout not found.");
            return Ok(result);
        }

        [HttpGet("payments")]
        public async Task<IActionResult> GetAllPaymentDetails([FromQuery] DateTime? dateTime = null)
        {
            var result = await checkoutService.GetAllPaymentDetalsAsync(dateTime);
            return Ok(result);
        }

        [HttpGet("payment-dates")]
        public async Task<IActionResult> GetAllPaymentDates()
        {
            var result = await checkoutService.GetAllPaymentDatesAsync();
            return Ok(result);
        }
    }
}
