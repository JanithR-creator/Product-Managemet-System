namespace CheckoutService.Models.Dto.ResDtos
{
    public class PaymentDetailsResDto
    {
        public Guid PaymentRecordId { get; set; }
        public Guid CheckoutId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
    }
}
