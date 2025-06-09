namespace CartService.Models.Dtos.RequestDtos
{
    public class CartItemReqDto
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ExternalProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
    }
}
