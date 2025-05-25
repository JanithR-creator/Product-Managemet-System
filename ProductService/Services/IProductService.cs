using ProductService.Model.Dtos.RequestDtos;

namespace ProductService.Services
{
    public interface IProductService
    {
        Task<int> SaveProducts(string provider);
        void DeleteAllProducts();
    }
}
