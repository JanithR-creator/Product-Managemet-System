using System.ComponentModel.DataAnnotations.Schema;

namespace CheckoutService.Models.Dto.ReqDtos
{
    public class CheckoutItemReqDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public Guid ExternalProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
    }
}
