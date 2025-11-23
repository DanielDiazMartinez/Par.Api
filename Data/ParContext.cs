using Microsoft.EntityFrameworkCore;
using Par.Api.Entities; 
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Par.Api.Data
{
    public class ParContext : DbContext
    {
        public ParContext(DbContextOptions<ParContext> options) : base(options)
        {
        }

        public DbSet<Box> Boxes { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Box>(entity =>
            {

                
                //This allows ef core to write when it fetches data from the database, but it is not accessible from outside the class.
                entity.Property(b => b.IsValid)
                      .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);

                entity.Property(b => b.TotalWeight)
                      .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);

               
                // Indexes on frequently queried fields to enhance read performance.
                entity.HasIndex(b => b.IsValid);
                entity.HasIndex(b => b.CreationDate); 
            });

            modelBuilder.Entity<Product>(entity =>
            {
                
                //Deleting a Box will also delete its associated Products.
                entity.HasOne(p => p.Box)
                      .WithMany(b => b.Products)
                      .HasForeignKey(p => p.BoxId)
                      .OnDelete(DeleteBehavior.Cascade);

                //Conversión enum → string to store ProductType as string in the database.
                entity.Property(p => p.Type).HasConversion<string>();
            });
        }
    }
}