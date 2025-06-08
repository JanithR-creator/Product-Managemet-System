using Common.Events;
using ProductService.Model.Dtos.RequestDtos;
using ProductService.Model.Dtos.ResponseDtos;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<int> SaveProducts();
        Task<bool> ReserveProductStockAsync(Guid productId, int quantity);
        Task<List<string>> GetAllCategoriesAsync();
        Task<ProductPaginateResDto> GetProductsAsync(int page, int pageSize, string? filter = null);
        Task<List<string>> GetAllProvidersAsync();
        Task<List<ProductResDto>> GetAllProductsAsync(string? provider = null, string? filter = null);
        void CreateInternalProduct(InternalProductReqDto dto);
        void UpdateProductInternalAsync(InternalProductReqDto dto, Guid productId);
        void DeleteInternalProductAsync(Guid productId);
        void DeleteAllProducts();
    }
}
