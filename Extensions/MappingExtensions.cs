using Par.Api.DTOs;
using Par.Api.Entities;

namespace Par.Api.Extensions
{
    public static class MappingExtensions
    {
    
        public static BoxDto ToDto(this Box box)
        {
            return new BoxDto
            {
                Id = box.Id,
                Size = box.Size,
                Author = box.Author,
                CreationDate = box.CreationDate,
                TotalWeight = box.TotalWeight,
                IsValid = box.IsValid,        
                Products = box.Products.Select(p => p.ToDto()).ToList()
            };
        }

        public static ProductDto ToDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Type = product.Type,
                Weight = product.Weight,
                Quantity = product.Quantity
            };
        }

      
        public static Box ToEntity(this CreateBoxDto dto)
        {
            return new Box
            {
                Size = dto.Size,
                CustomWeight = dto.CustomWeight,
                WeightRangeMin = dto.WeightRangeMin,
                WeightRangeMax = dto.WeightRangeMax,
                Author = dto.Author,
               
                Products = dto.Products.Select(p => new Product
                {
                    Name = p.Name,
                    Type = p.Type,
                    Weight = p.Weight,
                    Quantity = p.Quantity
                }).ToList()
            };
        }
    }
}