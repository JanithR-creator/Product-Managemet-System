using ProductService.Model.Dtos.RequestDtos;

namespace ProductService.AdapterEndPointController
{
    public interface IAdapterEnpointHandler
    {
        Task<List<ProductReqDto>> GetProductsListAsync(string provider);
    }
}
