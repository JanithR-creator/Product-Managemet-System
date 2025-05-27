namespace AdapterService.Models.Dtos.ExternalDtos
{
    public class NovelsDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public string Author { get; set; }= string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int PublishedYear { get; set; }
        public bool IsBestSeller { get; set; }

    }
}
