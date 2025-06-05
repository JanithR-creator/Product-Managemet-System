using CheckoutService.Models.Dto.ReqDtos;
using CheckoutService.Models.Dto.ResDtos;
namespace CheckoutService.Services
{
    public interface ICheckoutService
    {
        Task<CheckoutSuccessResDto> CreateCheckoutAsync(CheckoutReqDto dto);
        Task<bool> MakePaymentAsync(PaymentReqDto dto);
        Task<CheckoutResDto> GetCheckOutByCheckoutIdAsync(Guid checkOutId);
        Task<List<PaymentDetailsResDto>> GetAllPaymentDetalsAsync(DateTime? dateTime = null);
        Task<List<DateTime>> GetAllPaymentDatesAsync();
    }
}
