using CheckoutService.Models.Dto.ReqDtos;
using CheckoutService.Models.Dto.ResDtos;
namespace CheckoutService.Services
{
    public interface ICheckoutService
    {
        Task<CheckoutSuccessResDto> CreateCheckoutAsync(CheckoutReqDto dto);
        Task<bool> MakePaymentAsync(string provider, PaymentReqDto dto);
        Task<CheckoutResDto> GetCheckOutByUserIdAsync(Guid userId);
        Task<List<CheckoutResDto>> GetCheckoutsAsync();
    }
}
