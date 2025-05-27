namespace Common.Events
{
    public class ProductRestoreEvent
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid UserId { get; set; }
        public string Provider { get; set; } = string.Empty;
    }
}
