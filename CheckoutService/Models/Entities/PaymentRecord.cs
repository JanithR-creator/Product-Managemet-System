namespace CheckoutService.Models.Entities
{
    public class PaymentRecord
    {
        public Guid PaymentRecordId { get; set; }
        public Guid CheckoutId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
        public Checkout Checkout { get; set; } = null!;
    }
}
