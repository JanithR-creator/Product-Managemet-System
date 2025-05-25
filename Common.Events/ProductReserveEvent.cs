namespace Common.Events
{
    public class ProductReserveEvent
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
