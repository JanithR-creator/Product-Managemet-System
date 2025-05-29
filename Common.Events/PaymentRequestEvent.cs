namespace Common.Events
{
    public class PaymentRequestEvent
    {
        public Guid CheckoutId { get; set; }
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
