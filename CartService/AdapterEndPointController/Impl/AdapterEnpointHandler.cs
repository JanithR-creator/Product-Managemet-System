using CartService.Models.Dtos.ExternalCartDtos;

namespace CartService.AdapterEndPointController.Impl
{
    public class AdapterEnpointHandler : IAdapterEndPointHandler
    {
        public async Task<bool> AddToCartAsync(string provider, CartReqDto dto)
        {
            using var httpClient = new HttpClient();
            var adapterUrl = $"http://adapterservice:80/api/ProductAdapter?provider={provider}";

            var response = await httpClient.PostAsJsonAsync(adapterUrl, dto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveFromCartAsync(string provider, CartItemRemoveReqDto dto)
        {
            using var httpClient = new HttpClient();
            var adapterUrl = $"http://adapterservice:80/api/ProductAdapter?provider={provider}";

            var jsonContent = JsonContent.Create(dto);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(adapterUrl),
                Content = jsonContent
            };

            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateItemAsync(string provider, CartReqDto dto)
        {
            using var httpClient = new HttpClient();
            var adapterUrl = $"http://adapterservice:80/api/ProductAdapter?provider={provider}";

            var response = await httpClient.PutAsJsonAsync(adapterUrl, dto);
            return response.IsSuccessStatusCode;
        }
    }
}
