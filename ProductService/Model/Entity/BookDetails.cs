namespace ProductService.Model.Entity
{
    public class BookDetails
    {
        public Guid detailId { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public Product? Product { get; set; }
        public Guid ProductId { get; set; }
    }
}
