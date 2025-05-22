using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Model.Entity
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string provider { get; set; }=string.Empty;
        public string Name { get; set; }=string.Empty;
        public string Auther {  get; set; }=string.Empty;
        public string Publisher {  get; set; }=string.Empty;
        public string Description { get; set; }=string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;
        public int Quantity {  get; set; } = 0;
        public Category? Category {  get; set; }
        public Guid CategoryId { get; set; }
    }
}
