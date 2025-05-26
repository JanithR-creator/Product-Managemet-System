using AdapterService.Models.Dtos.InternalDtos;

namespace AdapterService.Services.AdapterService
{
    public interface IProductAdapter
    {
        string AdapterKey { get; }
        Task<List<Product>> GetProductsAsync();

        Task<bool> AddToCart(CartReqDto dto);
    }
}
