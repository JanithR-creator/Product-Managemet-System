namespace CartService.Models.Enitity
{
    public class Cart
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public List<CartItem> Items { get; set; } = new();
    }
}
