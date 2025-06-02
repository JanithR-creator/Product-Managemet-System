namespace CheckoutService.Models.Dto.ResDtos
{
    public class CheckoutSuccessResDto
    {
        public Guid CheckoutId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
