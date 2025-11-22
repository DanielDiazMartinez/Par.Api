using System.ComponentModel.DataAnnotations;
using Par.Api.Enums;

namespace Par.Api.DTOs
{
    public class UpdateBoxDto
    {
        [Required(ErrorMessage = "The size is required.")]
        public BoxSize Size { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "The custom weight must be greater than or equal to 0.")]
        public double CustomWeight { get; set; }

        public double? WeightRangeMin { get; set; }
        public double? WeightRangeMax { get; set; }

        [Required(ErrorMessage = "The author is required.")]
        public string Author { get; set; } = string.Empty;

        [Required(ErrorMessage = "At least one product must be included.")]
        [MinLength(1, ErrorMessage = "At least one product must be included.")]
        public List<CreateProductDto> Products { get; set; } = new List<CreateProductDto>();
    }
}