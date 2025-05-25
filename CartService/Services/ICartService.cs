using CartService.Models.Dtos.RequestDtos;

namespace CartService.Services
{
    public interface ICartService
    {
        Task AddItemToCart(CartItemReqDto dto);
    }
}
