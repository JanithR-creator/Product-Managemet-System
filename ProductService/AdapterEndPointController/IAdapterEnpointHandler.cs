using ProductService.Model.Dtos.RequestDtos;

namespace ProductService.AdapterEndPointController
{
    public interface IAdapterEnpointHandler
    {
        Task<bool> AddToCartAsync(string provider, CartReqDto dto);
        Task<bool> RemoveFromCartAsync(string provider, ExtCartItemRemoveDto dto);
        Task<bool> UpdateItemAsync(string provider, CartReqDto dto);
    }
}
