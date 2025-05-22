using AdapterService.Services.AdapterService;

namespace AdapterService.Services.FactoryService
{
    public interface IProductFactoryService
    {
        IProductAdapter Factory(string adapterKey);
    }
}
