using AdapterService.Services.AdapterService;

namespace AdapterService.Services.FactoryService.FactoryServiceImpl
{
    public class ProductFactoryService : IProductFactoryService
    {
        private readonly Dictionary<string, IProductAdapter> adapterMap; //to fast access the correct adapter using key.Efficient lookup (O(1)).

        public ProductFactoryService(IEnumerable<IProductAdapter> adapters) //adapter receives all regesterd adapter implementations
        {
            adapterMap = adapters.ToDictionary(a => a.AdapterKey.ToLower()); //Converts the list of adapters into a dictionary:Value: the adapter object itself.
        }

        public IProductAdapter Factory(string adapterKey)
        {
            return adapterMap.TryGetValue(adapterKey.ToLower(), out var adapter)
            ? adapter
            : throw new KeyNotFoundException($"No adapter found for provider '{adapterKey}'");
        }
    }
}
