using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Model.Dtos.ResponseDtos
{
    public class SchoolItemResDto
    {
        public Guid ProductId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public int Quantity { get; set; } = 0;
    }
}
