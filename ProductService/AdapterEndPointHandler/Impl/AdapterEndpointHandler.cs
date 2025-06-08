using ProductService.Model.Dtos.RequestDtos;

namespace ProductService.AdapterEndPointController.Impl
{
    public class AdapterEndpointHandler : IAdapterEnpointHandler
    {
        public async Task<List<ProductReqDto>> GetProductsListAsync()
        {
            using var httpClient = new HttpClient();
            var adapterUrl = $"http://adapterservice:80/api/ProductAdapter"; // service name in Docker

            var products = await httpClient.GetFromJsonAsync<List<ProductReqDto>>(adapterUrl);

            // Fix for CS8604: Ensure 'products' is not null before calling ToList()
            return products?.ToList() ?? new List<ProductReqDto>();
        }
    }
}
