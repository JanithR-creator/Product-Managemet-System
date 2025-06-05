namespace ProductService.Model.Dtos.RequestDtos
{
    public class InternalProductReqDto
    {
        public string Provider { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public string PruductType { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Author { get; set; }
        public string? Publisher { get; set; }
        public string? Category { get; set; }
    }
}
