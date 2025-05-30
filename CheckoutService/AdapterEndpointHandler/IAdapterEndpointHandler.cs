using CheckoutService.Models.Dto.ExternalDtos;

namespace CheckoutService.AdapterEndpointHandler
{
    public interface IAdapterEndpointHandler
    {
        Task<bool> MakePaymentAaync(ExtPaymentReqDto dto, string provider);
    }
}
