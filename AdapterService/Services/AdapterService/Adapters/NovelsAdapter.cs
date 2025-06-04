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

            if (novels == null) return new List<Product>();

            var internalProducts = novels.Select(n => new Product
            {
                ProductId = n.ProductId,
                Provider = "novels provider",
                Name = n.Name,
                Description = n.Description,
                Price = n.Price,
                Quantity = n.Quantity,
                PruductType = "novel",
                ImageUrl = n.ImageUrl,
                Author = n.Author,
                Publisher = n.Publisher,
                Category = n.Category
            }).ToList();

            return internalProducts;
        }

        public async Task<bool> AddToCartAsync(CartReqDto dto)
        {
            var externalUrl = "http://host.docker.internal:6000/add-to-cart";

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
            var externalUrl = "http://host.docker.internal:6000/remove-from-cart";

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
            var externalUrl = "http://host.docker.internal:6000/update-cart";
            var payload = new
            {
                userId = dto.UserId,
                productId = dto.ProductId,
                newQuantity = dto.Quantity
            };
            var response = await httpClient.PutAsJsonAsync(externalUrl, payload);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> MakePaymentAsync(PaymentReqDto dto)
        {
            var externalUrl = "http://host.docker.internal:6000/make-payment";

            var payload = new
            {
                userId = dto.UserId,
                paymentMethod = dto.PaymentMethod,
                totalAmount = dto.TotalAmount
            };

            var response = await httpClient.PostAsJsonAsync(externalUrl, payload);

            return response.IsSuccessStatusCode;
        }
    }
}
