namespace ProductService.Model.Dtos.ResponseDtos
{
    public class NovelResDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Guid ExternalProductID { get; set; }

        public string Author { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public string Publisher { get; set; } = "";
        public string Category { get; set; } = "";
    }
}
