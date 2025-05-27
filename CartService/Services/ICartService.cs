using CartService.Models.Dtos.RequestDtos;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Services
{
    public interface ICartService
    {
        Task AddItemToCart(CartItemReqDto dto, string provider);
        Task RemoveItemFromCart(Guid cartItemId, string provider);
        Task UpdateCartItem(CartItemUpdateReqDto dto, string provider);
    }
}
