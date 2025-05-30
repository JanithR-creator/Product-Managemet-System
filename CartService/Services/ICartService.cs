using CartService.Models.Dtos.RequestDtos;
using CartService.Models.Dtos.ResponseDtos;
using Common.Events;

namespace CartService.Services
{
    public interface ICartService
    {
        Task<bool> AddItemToCart(CartItemReqDto dto, string provider);
        Task RemoveItemFromCart(Guid cartItemId, string provider);
        Task UpdateCartItem(CartItemUpdateReqDto dto, string provider);
        Task<List<CartItemGetResDto>> GetCartItems(Guid userId);
        Task<List<CartDetailsResDto>> GetAllCartItems();
        Task<bool> Checkout(CheckoutEventDto @event);
    }
}
