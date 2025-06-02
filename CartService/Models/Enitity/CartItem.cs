using System.ComponentModel.DataAnnotations.Schema;

namespace CartService.Models.Enitity
{
    public class CartItem
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public Guid? ExternalProductId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public DateTime AddedDate { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = new();
    }
}