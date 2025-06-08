using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ProductService.Model.Entity
{
    public class ProductDetails
    {
        [Key]
        public Guid ProductId { get; set; }
        [Required]
        public string MetadataJson { get; set; } = string.Empty;
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }
    }
}
