namespace CartService.Models.Dtos.RequestDtos
{
    public class CartItemReqDto
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Quantity { get; set; }
    }
}
