using AmazeCare.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmazeCare.Server.Data.Configurations
{
    public class LabTestCatalogConfiguration : IEntityTypeConfiguration<LabTestCatalog>
    {
        public void Configure(EntityTypeBuilder<LabTestCatalog> builder)
        {
            builder.ToTable("LabTestCatalogs");

            builder.HasKey(tc => tc.TestCatalogId);

            builder.Property(tc => tc.TestName)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(tc => tc.TestName)
                .IsUnique();

            builder.Property(tc => tc.ShortName)
                .HasMaxLength(20);

            builder.Property(tc => tc.Category)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(tc => tc.NormalRange)
                .HasMaxLength(300);

            builder.Property(tc => tc.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}