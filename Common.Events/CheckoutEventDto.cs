namespace Common.Events
{
    public class CheckoutEventDto
    {
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
