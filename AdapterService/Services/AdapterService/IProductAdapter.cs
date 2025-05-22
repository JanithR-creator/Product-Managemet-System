using AdapterService.Models;

namespace AdapterService.Services.AdapterService
{
    public interface IProductAdapter
    {
        string AdapterKey { get; }
        Task<List<Product>> GetProductsAsync();
    }
}
