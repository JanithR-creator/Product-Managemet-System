namespace CheckoutService.Models.Dto.ReqDtos
{
    public class CheckoutReqDto
    {
        public Guid UserId { get; set; }
        public List<CheckoutItemReqDto> Items { get; set; } = new();
    }
}
