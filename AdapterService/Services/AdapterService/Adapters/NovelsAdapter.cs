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
                Provider = "novel",
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

        public async Task<bool> MakePaymentAsync(PaymentReqDto dto)
        {
            var externalUrl = "http://host.docker.internal:6000/make-payment";

            var payload = new
            {
                userId = dto.UserId,
                paymentMethod = dto.PaymentMethod,
                totalAmount = dto.TotalAmount,
                products = dto.Products
            };

            var response = await httpClient.PostAsJsonAsync(externalUrl, payload);

            return response.IsSuccessStatusCode;
        }
    }
}
