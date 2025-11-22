using System.ComponentModel.DataAnnotations;
using Par.Api.Enums;

namespace Par.Api.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "The name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "The product type is required.")]
        public ProductType Type { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "The weight must be greater than 0.")]
        public double Weight { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}