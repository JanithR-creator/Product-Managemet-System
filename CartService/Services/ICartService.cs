using CartService.Models.Dtos.RequestDtos;
using CartService.Models.Dtos.ResponseDtos;
using Common.Events;

namespace CartService.Services
{
    public interface ICartService
    {
        Task<bool> AddItemToCart(CartItemReqDto dto);
        Task RemoveItemFromCart(Guid cartItemId);
        Task UpdateCartItem(CartItemUpdateReqDto dto);
        Task<List<CartItemGetResDto>> GetCartItems(Guid userId);
        Task<bool> Checkout(CheckoutEventDto @event);
    }
}
