namespace Common.Events
{
    public class ProductReserveEvent
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Quantity { get; set; }
        public string Provider { get; set; } = string.Empty;
    }
}
