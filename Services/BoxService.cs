using Microsoft.EntityFrameworkCore;
using Par.Api.Data;
using Par.Api.DTOs;
using Par.Api.Enums;
using Par.Api.Extensions;
using Par.Api.Entities;
using System.Text; 

namespace Par.Api.Services
{
    public class BoxService
    {
        private readonly ParContext _context;

       
        public BoxService(ParContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BoxDto>> GetAllBoxesAsync(int pageNumber, int pageSize)
        {
            var boxes = await _context.Boxes
                .AsNoTracking()
                .Include(b => b.Products)
                .OrderByDescending(b => b.CreationDate)
                .Skip((pageNumber - 1) * pageSize) 
                .Take(pageSize)                   
                .ToListAsync();

            return boxes.Select(b => b.ToDto());
        }

        public async Task<BoxDto?> GetBoxByIdAsync(int id)
        {
            var box = await _context.Boxes
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);

            return box?.ToDto(); 
        }


        public async Task<BoxDto> CreateBoxAsync(CreateBoxDto createDto)
        {
            var box = new Box
            {
                Size = createDto.Size,
                CustomWeight = createDto.CustomWeight,
                WeightRangeMin = createDto.WeightRangeMin,
                WeightRangeMax = createDto.WeightRangeMax,
                Author = createDto.Author,
                CreationDate = DateTime.UtcNow,
                Products = createDto.Products.Select(p => new Product
                {
                    Name = p.Name,
                    Type = p.Type,
                    Weight = p.Weight,
                    Quantity = p.Quantity
                }).ToList()
            };

            box.UpdateValidity();
            
            _context.Boxes.Add(box);
            await _context.SaveChangesAsync();
            
   
            await _context.Entry(box)
                .Collection(b => b.Products)
                .LoadAsync();

            return box.ToDto();
        }

   
        public async Task<BoxDto?> UpdateBoxAsync(int id, UpdateBoxDto updateDto)
        {
            var box = await _context.Boxes
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (box == null)
                return null;

     
            box.Size = updateDto.Size;
            box.CustomWeight = updateDto.CustomWeight;
            box.WeightRangeMin = updateDto.WeightRangeMin;
            box.WeightRangeMax = updateDto.WeightRangeMax;
            box.Author = updateDto.Author;

            _context.Products.RemoveRange(box.Products);

            box.Products = updateDto.Products.Select(p => new Product
            {
                Name = p.Name,
                Type = p.Type,
                Weight = p.Weight,
                Quantity = p.Quantity,
                BoxId = box.Id
            }).ToList();

     
            box.UpdateValidity();

            await _context.SaveChangesAsync();

            return box.ToDto();
        }

 
        public async Task<BoxDto?> PatchBoxAsync(int id, PatchBoxDto patchDto)
        {
            var box = await _context.Boxes
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (box == null)
                return null;

   
            if (patchDto.Size.HasValue)
                box.Size = patchDto.Size.Value;

            if (patchDto.CustomWeight.HasValue)
                box.CustomWeight = patchDto.CustomWeight.Value;

            if (patchDto.WeightRangeMin.HasValue)
                box.WeightRangeMin = patchDto.WeightRangeMin;

            if (patchDto.WeightRangeMax.HasValue)
                box.WeightRangeMax = patchDto.WeightRangeMax;

            if (!string.IsNullOrWhiteSpace(patchDto.Author))
                box.Author = patchDto.Author;

            box.UpdateValidity();

            await _context.SaveChangesAsync();

            return box.ToDto();
        }


        public async Task<bool> DeleteBoxAsync(int id)
        {
            var box = await _context.Boxes
                .FirstOrDefaultAsync(b => b.Id == id);

            if (box == null)
                return false;

            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<Stream> GetBoxesExportStreamAsync()
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, new UTF8Encoding(true), bufferSize: 1024, leaveOpen: true);

           
            await writer.WriteLineAsync("ID;Author;Size;IsValid;Total Weight;Weight Min;Weight Max;Product Count;Date");

            int batchSize = 1000;
            int pageNumber = 0;
            bool continueExporting = true;

            while (continueExporting)
            {
                var batch = await _context.Boxes
                    .AsNoTracking()
                    .Skip(pageNumber * batchSize)
                    .Take(batchSize)
                    .Include(b => b.Products) 
                    .ToListAsync();

                if (!batch.Any())
                {
                    continueExporting = false;
                    continue;
                }

                foreach (var box in batch)
                {
                    
                    var weightMin = box.WeightRangeMin.HasValue ? box.WeightRangeMin.Value.ToString("F2") : "";
                    var weightMax = box.WeightRangeMax.HasValue ? box.WeightRangeMax.Value.ToString("F2") : "";
                    
                    var line = $"{box.Id};{EscapeCsv(box.Author)};{box.Size};{box.IsValid};{box.TotalWeight:F2};{weightMin};{weightMax};{box.Products.Count};{box.CreationDate:yyyy-MM-dd HH:mm}";
                    await writer.WriteLineAsync(line);
                }

                pageNumber++;
            }

            await writer.FlushAsync();
            stream.Position = 0; 
            return stream;
        }

        public async Task<(byte[] content, string fileName)> ExportBoxesToCsvAsync()
        {
            var stream = await GetBoxesExportStreamAsync();
            using (stream)
            {
                var fileContent = new byte[stream.Length];
                await stream.ReadAsync(fileContent, 0, (int)stream.Length);
                var fileName = $"Boxes_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                return (fileContent, fileName);
            }
        }

        private string EscapeCsv(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            
            if (input.Contains(";")) return $"\"{input}\"";
            return input;
        }
    }
}