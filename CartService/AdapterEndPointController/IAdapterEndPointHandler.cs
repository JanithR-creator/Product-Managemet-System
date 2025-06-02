using CartService.Models.Dtos.ExternalCartDtos;

namespace CartService.AdapterEndPointController
{
    public interface IAdapterEndPointHandler
    {
        Task<bool> AddToCartAsync(string provider, CartReqDto dto);
        Task<bool> RemoveFromCartAsync(string provider, CartItemRemoveReqDto dto);
        Task<bool> UpdateItemAsync(string provider, CartReqDto dto);
    }
}
