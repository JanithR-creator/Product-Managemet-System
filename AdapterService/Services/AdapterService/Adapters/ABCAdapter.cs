using AdapterService.Models;
using AdapterService.Models.Dtos.ReponseDtos;

namespace AdapterService.Services.AdapterService.AdapterServiceImpl
{
    public class ABCAdapter:IProductAdapter
    {
        public string AdapterKey => "abc";

        private readonly HttpClient httpClient;

        public ABCAdapter(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var externalUrl = "http://host.docker.internal:3001/products";

            var externalProducts = await httpClient.GetFromJsonAsync<List<ExternalProductRespDto>>(externalUrl);

            if (externalProducts == null) return new List<Product>();

            var internalProduct = externalProducts.Select(p =>new Product
            {
                Name=p.Title,
                Description=p.Description,
                Price=p.Price,
            }).ToList();

            return internalProduct;
        }
    }
}
