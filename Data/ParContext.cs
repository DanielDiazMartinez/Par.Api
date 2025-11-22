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
                /// <summary>
                /// PropertyAccessMode.FieldDuringConstruction allows EF Core to directly access
                /// the private field during entity construction, but use the public setter
                /// afterwards.
                /// </summary>
                entity.Property(b => b.IsValid)
                      .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);

                entity.Property(b => b.TotalWeight)
                      .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);

                /// <summary>
                /// Indexes on frequently queried fields to enhance read performance.
                /// </summary>
                entity.HasIndex(b => b.IsValid);
                entity.HasIndex(b => b.CreationDate); 
            });

            modelBuilder.Entity<Product>(entity =>
            {
                /// <summary>
                /// Deleting a Box will also delete its associated Products.
                /// </summary>
                entity.HasOne(p => p.Box)
                      .WithMany(b => b.Products)
                      .HasForeignKey(p => p.BoxId)
                      .OnDelete(DeleteBehavior.Cascade);

                /// <summary>
                /// Conversión enum → string to store ProductType as string in the database.
                /// </summary>
                entity.Property(p => p.Type).HasConversion<string>();
            });
        }
    }
}