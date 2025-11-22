using Par.Api.Enums;
using System.ComponentModel.DataAnnotations;

namespace Par.Api.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ProductType Type { get; set; }
        public double Weight { get; set; }
        public int Quantity { get; set; }
    }
}
