using CartService.Models.Dtos.RequestDtos;
using CartService.Models.Dtos.ResponseDtos;

namespace CartService.Services
{
    public interface ICartService
    {
        Task AddItemToCart(CartItemReqDto dto, string provider);
        Task RemoveItemFromCart(Guid cartItemId, string provider);
        Task UpdateCartItem(CartItemUpdateReqDto dto, string provider);
        Task<List<CartItemGetResDto>> GetCartItems(Guid userId);
        Task<List<CartDetailsResDto>> GetAllCartItems();
    }
}
