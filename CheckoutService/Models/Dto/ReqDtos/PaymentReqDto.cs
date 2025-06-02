namespace CheckoutService.Models.Dto.ReqDtos
{
    public class PaymentReqDto
    {
        public Guid CheckoutId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
    }
}
