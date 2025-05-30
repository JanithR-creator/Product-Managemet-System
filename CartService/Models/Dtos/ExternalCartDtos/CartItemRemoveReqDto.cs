namespace CartService.Models.Dtos.ExternalCartDtos
{
    public class CartItemRemoveReqDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
