using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Model.Entity
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public Guid ExternalDbId { get; set; }
        public string Provider { get; set; }=string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public string PruductType { get; set; } = string.Empty;
        public BookDetails? BookDetails { get; set; }
    }
}
