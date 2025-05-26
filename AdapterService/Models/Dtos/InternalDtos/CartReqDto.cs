namespace AdapterService.Models.Dtos.InternalDtos
{
    public class CartReqDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
