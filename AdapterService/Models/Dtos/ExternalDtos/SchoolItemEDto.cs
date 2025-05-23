using System.ComponentModel.DataAnnotations.Schema;

namespace AdapterService.Models.Dtos.ReponseDtos
{
    public class SchoolItemEDto
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public string Category {  get; set; } = string.Empty;
        public DateTime CreatedDate {  get; set; }
        public bool IsAvailable { get; set; }
    }
}
