using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Par.Api.Enums;


namespace Par.Api.Entities
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public ProductType Type { get; set; } 

        [Required]
        public double Weight { get; set; } 

        [Required]
        public int Quantity { get; set; }

        public int BoxId { get; set; } 

        [ForeignKey("BoxId")]
        public virtual Box? Box { get; set; }
    }
}