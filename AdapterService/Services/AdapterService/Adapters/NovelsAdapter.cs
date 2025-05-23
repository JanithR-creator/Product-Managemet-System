using AdapterService.Models.Dtos.ExternalDtos;
using AdapterService.Models.Dtos.InternalDtos;

namespace AdapterService.Services.AdapterService.Adapters
{
    public class NovelsAdapter : IProductAdapter
    {
        public string AdapterKey => "novel";

        private readonly HttpClient httpClient;

        public NovelsAdapter(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var url = "http://host.docker.internal:3002/novels";

            var novels = await httpClient.GetFromJsonAsync<List<NovelsDto>>(url);

            if(novels == null) return new List<Product>();

            var internalProducts = novels.Select(n => new Product
            {
                Provider = "novel",
                Name = n.Name,
                Description = n.Description,
                Price = n.Price,
                Quantity = n.Quantity,
                PruductType = "novel",
                Author = n.Author,
                Publisher = n.Publisher,
                Category = n.Category
            }).ToList();

            return internalProducts;
        }
    }
}
