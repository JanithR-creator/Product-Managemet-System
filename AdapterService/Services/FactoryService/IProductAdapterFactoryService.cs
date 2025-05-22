using AdapterService.Services.AdapterService;

namespace AdapterService.Services.FactoryService
{
    public interface IProductAdapterFactoryService
    {
        IProductAdapter Factory(string adapterKey);
    }
}
