using Common.Events;
using ProductService.Model.Dtos.RequestDtos;
using ProductService.Model.Dtos.ResponseDtos;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<int> SaveProducts(string provider);
        Task<bool> ReserveProductStockAsync(ProductCommonEventDto @event);
        Task<bool> RestoreProductStockAsync(ProductCommonEventDto @event);
        Task<bool> UpdateProductStockAsync(ProductCommonEventUpdateDto @event);
        Task<List<string>> GetAllCategoriesAsync();
        Task<ProductPaginateResDto> GetProductsAsync(string productType, int page, int pageSize, string? filter = null);
        Task<List<string>> GetAllProvidersAsync();
        Task<List<ProductResDto>> GetAllProductsAsync(string? provider = null, string? filter = null);
        void CreateInternalProduct(InternalProductReqDto dto);
        void UpdateProductInternalAsync(InternalProductReqDto dto, Guid productId);
        void DeleteInternalProductAsync(Guid productId);
        void DeleteAllProducts();
    }
}
