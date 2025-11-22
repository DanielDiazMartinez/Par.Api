using Par.Api.Enums;
using System.ComponentModel.DataAnnotations;
namespace Par.Api.DTOs
{
    public class BoxDto
    {
        public int Id { get; set; }
        public BoxSize Size { get; set; }
        public string Author { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public double TotalWeight { get; set; }
        public bool IsValid { get; set; }

        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
