using System.ComponentModel.DataAnnotations;
using Par.Api.Enums;

namespace Par.Api.DTOs
{
    public class PatchBoxDto
    {
        public BoxSize? Size { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "The custom weight must be greater than or equal to 0.")]
        public double? CustomWeight { get; set; }

        public double? WeightRangeMin { get; set; }
        public double? WeightRangeMax { get; set; }

        public string? Author { get; set; }
    }
}