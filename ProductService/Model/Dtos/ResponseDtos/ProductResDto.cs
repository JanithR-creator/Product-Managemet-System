namespace ProductService.Model.Dtos.ResponseDtos
{
    public class ProductResDto
    {
        public Guid ProductId { get; set; }
        public Guid ExternalProductID { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Provider { get; set; } = "";
        public string ProductType { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
