namespace AdapterService.Models.Dtos.InternalDtos
{
    public class ItemRemoveReqDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
