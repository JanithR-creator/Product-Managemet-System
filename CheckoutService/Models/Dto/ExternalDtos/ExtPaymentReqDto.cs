namespace CheckoutService.Models.Dto.ExternalDtos
{
    public class ExtPaymentReqDto
    {
        public Guid UserId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public List<ExtProductDetailDto> Products { get; set; } = new List<ExtProductDetailDto>();
    }
}
