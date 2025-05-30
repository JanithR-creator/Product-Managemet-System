namespace AdapterService.Models.Dtos.InternalDtos
{
    public class PaymentReqDto
    {
        public Guid UserId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
