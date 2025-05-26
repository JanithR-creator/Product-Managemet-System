using AdapterService.Models.Dtos.InternalDtos;
using AdapterService.Models.Dtos.ReponseDtos;

namespace AdapterService.Services.AdapterService.AdapterServiceImpl
{
    public class SchoolItemAdapter:IProductAdapter
    {
        public string AdapterKey => "school item";

        private readonly HttpClient httpClient;

        public SchoolItemAdapter(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var externalUrl = "http://host.docker.internal:3001/products";

            var externalProducts = await httpClient.GetFromJsonAsync<List<SchoolItemEDto>>(externalUrl);

            if (externalProducts == null) return new List<Product>();

            var internalProduct = externalProducts.Select(p =>new Product
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                PruductType="school item",
                Provider= "school items provider"
            }).ToList();

            return internalProduct;
        }

        public async Task<bool> AddToCart(CartReqDto dto)
        {
            var externalUrl = "http://host.docker.internal:5000/add-to-cart";

            var payload = new 
            {
                userId = dto.UserId,
                productId = dto.ProductId,
                quantity = dto.Quantity
            };

            var response = await httpClient.PostAsJsonAsync(externalUrl, payload);

            return response.IsSuccessStatusCode;
        }
    }
}
