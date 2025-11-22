using Microsoft.EntityFrameworkCore;
using Par.Api.Entities;
using Par.Api.Enums;
using System.Diagnostics;

namespace Par.Api.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<ParContext>();

            if (await context.Boxes.AnyAsync())
            {
                return; 
            }

            Console.WriteLine("--> Seeding 50,000 records... Please wait.");

            var stopwatch = Stopwatch.StartNew();
            var random = Random.Shared; // Usar Random.Shared para mejor aleatoriedad
            var boxesToAdd = new List<Box>(50000); 
            var users = new[] { "Manager", "Operator" };

            for (int i = 0; i < 50000; i++)
            {
                var products = new List<Product>();
                int productCount = random.Next(1, 5);

                for (int j = 0; j < productCount; j++)
                { 
                    products.Add(new Product
                    {
                        Name = $"Product {i}-{j}",
                        Type = (ProductType)random.Next(0, 3),
                        Weight = random.NextDouble() * 10 + 1, 
                        Quantity = random.Next(1, 5)
                    });
                }

               
                var selectedAuthor = users[random.Next(users.Length)];
                
               
                int validHour;
                if (selectedAuthor == "Manager")
                    validHour = random.Next(8, 12); 
                else if (selectedAuthor == "Operator")
                    validHour = random.Next(12, 17); 
                else
                    validHour = random.Next(0, 24); 

                var box = new Box
                {
                    Size = (BoxSize)random.Next(0, 3), 
                    CustomWeight = random.NextDouble() * 5,
                    Author = selectedAuthor,
                    
                    
                    CreationDate = DateTime.UtcNow.Date
                        .AddHours(validHour)
                        .AddMinutes(random.Next(0, 60)),
                    
                    Products = products,
                    WeightRangeMin = random.Next(0, 2) == 0 ? random.Next(5, 50) : null,
                    WeightRangeMax = random.Next(0, 2) == 0 ? random.Next(100, 2000) : null
                };

                box.UpdateValidity();
                boxesToAdd.Add(box);
            }

            await context.Boxes.AddRangeAsync(boxesToAdd);
            await context.SaveChangesAsync();

            stopwatch.Stop();
            Console.WriteLine($"Inserted 50,000 boxes in {stopwatch.ElapsedMilliseconds}ms.");


        }
    }
}