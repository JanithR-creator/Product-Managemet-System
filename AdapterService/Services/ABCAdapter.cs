using AdapterService.Models;
using AdapterService.Models.Dtos;
using AdapterService.Services.Interfeces;

namespace AdapterService.Services
{
    public class ABCAdapter:IProductAdapter
    {
        private readonly HttpClient httpClient;

        public ABCAdapter(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var externalUrl = "http://host.docker.internal:3001/products";

            var externalProducts = await this.httpClient.GetFromJsonAsync<List<ExternalProductRespDto>>(externalUrl);

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
