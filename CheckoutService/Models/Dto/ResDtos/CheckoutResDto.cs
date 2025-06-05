namespace CheckoutService.Models.Dto.ResDtos
{
    public class CheckoutResDto
    {
        public Guid CheckoutId { get; set; }
        public Guid PaymentRecordId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CheckoutDate { get; set; }
        public DateTime PaidDate { get; set; }
        public string CheckoutStatus { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public List<CheckoutItemResDto> Items { get; set; } = new List<CheckoutItemResDto>();
    }
}
