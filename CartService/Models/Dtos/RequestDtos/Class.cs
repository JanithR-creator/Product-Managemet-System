namespace CartService.Models.Dtos.RequestDtos
{
    public class CartItemRemoveReqDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
