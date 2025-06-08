using AdapterService.Services.AdapterService;

namespace AdapterService.Services.FactoryService.FactoryServiceImpl
{
    public class ProductFactoryService : IProductFactoryService
    {
        private readonly Dictionary<string, IProductAdapter> adapterMap;

        public ProductFactoryService(IEnumerable<IProductAdapter> adapters)
        {
            adapterMap = adapters.ToDictionary(a => a.AdapterKey.ToLower());
        }

        public IProductAdapter Factory(string adapterKey)
        {
            return adapterMap.TryGetValue(adapterKey.ToLower(), out var adapter)
            ? adapter
            : throw new KeyNotFoundException($"No adapter found for provider '{adapterKey}'");
        }

        public List<IProductAdapter> GetAllAdapters()
        {
            return adapterMap.Values.ToList();
        }
    }
}
