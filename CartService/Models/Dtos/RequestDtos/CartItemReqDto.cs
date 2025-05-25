namespace CartService.Models.Dtos.RequestDtos
{
    public class CartItemReqDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
