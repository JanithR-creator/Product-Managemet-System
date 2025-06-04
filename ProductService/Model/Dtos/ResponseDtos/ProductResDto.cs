namespace ProductService.Model.Dtos.ResponseDtos
{
    public class ProductResDto
    {
        public List<Object> Products { get; set; } = new List<object>();
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public int TotalItems { get; set; } = 0;
    }
}
