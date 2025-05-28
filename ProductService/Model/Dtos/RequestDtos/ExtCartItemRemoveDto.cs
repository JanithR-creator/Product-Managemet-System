namespace ProductService.Model.Dtos.RequestDtos
{
    public class ExtCartItemRemoveDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
