namespace CartService.Models.Dtos.RequestDtos
{
    public class CartItemUpdateReqDto
    {
        public Guid CartItemId { get; set; }
        public int Quantity { get; set; }
    }
}
