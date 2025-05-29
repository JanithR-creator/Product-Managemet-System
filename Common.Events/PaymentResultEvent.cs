namespace Common.Events
{
    public class PaymentResultEvent
    {
        public Guid CheckoutId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
