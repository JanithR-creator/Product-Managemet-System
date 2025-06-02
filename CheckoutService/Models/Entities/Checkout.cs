namespace CheckoutService.Models.Entities
{
    public class Checkout
    {
        public Guid CheckoutId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<CheckoutItem> Items { get; set; } = new();
        public PaymentRecord? Payment { get; set; }
    }
}
