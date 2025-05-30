using CheckoutService.Models.Dto.ExternalDtos;

namespace CheckoutService.AdapterEndpointHandler.Impl
{
    public class AdapterEndPointHandler : IAdapterEndpointHandler
    {
        public async Task<bool> MakePaymentAaync(ExtPaymentReqDto dto, string provider)
        {
            using var httpClient = new HttpClient();
            var adapterUrl = $"http://adapterservice:80/api/ProductAdapter/make-payment?provider={provider}";

            var response = await httpClient.PostAsJsonAsync(adapterUrl, dto);

            return response.IsSuccessStatusCode;
        }
    }
}
