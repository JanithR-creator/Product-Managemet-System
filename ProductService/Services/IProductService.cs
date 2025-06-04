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
        Task<ProductResDto> GetProductsAsync(string productType, int page, int pageSize, string? filter = null);
        void CreateInternalProduct(ProductReqDto dto);
        void DeleteAllProducts();
    }
}
