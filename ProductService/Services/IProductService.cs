using Common.Events;
using ProductService.Model.Dtos.ResponseDtos;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<int> SaveProducts(string provider);
        Task<bool> ReserveProductStockAsync(ProductReserveEvent @event);
        Task RestoreProductStockAsync(ProductRestoreEvent @event);
        Task<List<NovelResDto>> GetAllNovels(int page, int pageSize);
        Task<List<SchoolItemResDto>> GetAllSclItems(int page, int pageSize);
        void DeleteAllProducts();
    }
}
