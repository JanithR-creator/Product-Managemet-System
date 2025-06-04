using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Model.Dtos.ResponseDtos
{
    public class SchoolItemResDto
    {
        public Guid ProductId { get; set; }
        public Guid ExternalProductID { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
