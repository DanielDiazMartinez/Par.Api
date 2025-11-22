using System.ComponentModel.DataAnnotations;
using Par.Api.Enums;
using Par.Api.Extensions;
namespace Par.Api.Entities
{
    public class Box
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public BoxSize Size { get; set; }

        public double CustomWeight { get; set; }

        public double? WeightRangeMin { get; set; }
        public double? WeightRangeMax { get; set; }

        public DateTime CreationDate { get; set; } 

        [Required]
        public string Author { get; set; } = string.Empty;

    
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public double TotalWeight { get; private set; }
        public bool IsValid { get; private set; }
        /// <summary>
        /// Calculates and updates the validity of a box based on 3 business rules:
        /// 1. Total weight within the permitted range (WeightRangeMin/Max)
        /// 2. Authorized author according to creation time (Manager: 8 a.m. to 12 p.m., Operator: 12 p.m. to 5 p.m.)
        /// 3. All products must be of the same type
        /// </summary>
        public void UpdateValidity()
        {
            
            double productWeightSum = Products != null ? Products.Sum(p => p.Weight * p.Quantity) : 0;
            double sizeWeight = Size.GetFixedWeight(); 
            TotalWeight = productWeightSum + sizeWeight + CustomWeight;

           
            bool weightOk = true;
            if (WeightRangeMin.HasValue && TotalWeight < WeightRangeMin.Value) weightOk = false;
            if (WeightRangeMax.HasValue && TotalWeight > WeightRangeMax.Value) weightOk = false;

           
            bool authorOk = false;
            int hour = CreationDate.Hour;

            if (Author == "Manager")
            {
                authorOk = hour >= 8 && hour < 12;  
            }
            else if (Author == "Operator")
            {
                authorOk = hour >= 12 && hour < 17; 
            }

            
            bool typeOk = true;
            if (Products != null && Products.Any())
            {
                var firstType = Products.First().Type;
                if (Products.Any(p => p.Type != firstType)) typeOk = false;
            }

            
            IsValid = weightOk && authorOk && typeOk;
        }
    }
}