using AdapterService.Models;

namespace AdapterService.Services.Interfeces
{
    public interface IProductAdapter
    {
        Task<List<Product>> GetProductsAsync();
    }
}
