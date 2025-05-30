namespace CartService.Models.Dtos.ExternalCartDtos
{
    public class CartReqDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
