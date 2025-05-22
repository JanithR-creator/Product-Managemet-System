using AdapterService.Services.AdapterService;

namespace AdapterService.Services.FactoryService.FactoryServiceImpl
{
    public class ProductAdapterFactoryService : IProductAdapterFactoryService
    {
        private readonly Dictionary<string, IProductAdapter> adapterMap;

        public ProductAdapterFactoryService(IEnumerable<IProductAdapter> adapters)
        {
            adapterMap = adapters.ToDictionary(a => a.AdapterKey.ToLower());
        }

        public IProductAdapter Factory(string adapterKey)
        {
            return adapterMap.TryGetValue(adapterKey.ToLower(), out var adapter)
            ? adapter
            : throw new KeyNotFoundException($"No adapter found for provider '{adapterKey}'");
        }
    }
}
