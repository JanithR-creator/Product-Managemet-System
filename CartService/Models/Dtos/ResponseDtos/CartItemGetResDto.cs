namespace CartService.Models.Dtos.ResponseDtos
{
    public class CartItemGetResDto
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public DateTime AddedDate { get; set; }
    }
}
