using E_Commerce.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configurations
{
    internal class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(b => b.ProductBrand)
                   .WithMany()
                   .HasForeignKey(b => b.BrandId);

            builder.HasOne(b => b.ProductType)
                   .WithMany()
                   .HasForeignKey(b => b.TypeId);
            builder.Property(b => b.Name).HasColumnType("nvarchar(100)");
            builder.Property(b => b.Description).HasColumnType("nvarchar(500)");
            builder.Property(b => b.PictureUrl).HasColumnType("nvarchar(200)");
            builder.Property(b=> b.Price).HasColumnType("decimal(18,2)");
        }
    }
}
