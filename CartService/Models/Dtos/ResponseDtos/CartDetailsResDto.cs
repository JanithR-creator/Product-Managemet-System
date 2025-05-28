using CartService.Models.Enitity;

namespace CartService.Models.Dtos.ResponseDtos
{
    public class CartDetailsResDto
    {
        public Guid CartId { get; set; }
        public Guid UserId { get; set; }
        public List<CartItemGetResDto> Items { get; set; } = new();
    }
}
