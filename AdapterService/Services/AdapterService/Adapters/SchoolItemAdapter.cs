using AdapterService.Models.Dtos.InternalDtos;
using AdapterService.Models.Dtos.ReponseDtos;

namespace AdapterService.Services.AdapterService.AdapterServiceImpl
{
    public class SchoolItemAdapter : IProductAdapter
    {
        public string AdapterKey => "school-item";

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
                PruductType = "school-item",
                Provider = "school-item",
                ImageUrl = p.ImageUrl
            }).ToList();

            return internalProduct;
        }
        public async Task<bool> MakePaymentAsync(PaymentReqDto dto)
        {
            var externalUrl = "http://host.docker.internal:5000/make-payment";

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
