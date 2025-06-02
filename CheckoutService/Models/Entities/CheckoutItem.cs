namespace CheckoutService.Models.Entities
{
    public class CheckoutItem
    {
        public Guid CheckoutItemId { get; set; }
        public Guid CheckoutId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public Checkout Checkout { get; set; } = null!;
    }
}
