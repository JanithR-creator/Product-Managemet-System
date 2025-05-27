using AdapterService.Models.Dtos.InternalDtos;
using AdapterService.Models.Dtos.ReponseDtos;

namespace AdapterService.Services.AdapterService.AdapterServiceImpl
{
    public class SchoolItemAdapter : IProductAdapter
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

            var internalProduct = externalProducts.Select(p => new Product
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                PruductType = "school item",
                Provider = "school items provider"
            }).ToList();

            return internalProduct;
        }

        public async Task<bool> AddToCartAsync(CartReqDto dto)
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

        public async Task<bool> RemoveFromCartAsync(ItemRemoveReqDto dto)
        {
            var externalUrl = "http://host.docker.internal:5000/remove-from-cart";

            var payload = new
            {
                userId = dto.UserId,
                productId = dto.ProductId
            };

            // Serialize the payload to JSON
            var jsonContent = JsonContent.Create(payload);

            // Create a DELETE HttpRequestMessage
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(externalUrl),
                Content = jsonContent
            };

            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItemAsync(CartReqDto dto)
        {
            var externalUrl = "http://host.docker.internal:5000/update-cart";
            var payload = new
            {
                userId = dto.UserId,
                productId = dto.ProductId,
                newQuantity = dto.Quantity
            };
            var response = await httpClient.PutAsJsonAsync(externalUrl, payload);
            return response.IsSuccessStatusCode;
        }
    }
}
