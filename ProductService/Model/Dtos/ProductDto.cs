namespace ProductService.Model.Dtos
{
    public class ProductDto
    {
        public string Provider { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public string PruductType { get; set; } = string.Empty;
        public string Author { get; set; }= string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}
