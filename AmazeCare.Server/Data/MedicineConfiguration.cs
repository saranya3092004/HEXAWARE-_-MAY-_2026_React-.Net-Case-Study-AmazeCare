using AmazeCare.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmazeCare.Server.Data.Configurations
{
    public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
    {
        public void Configure(EntityTypeBuilder<Medicine> builder)
        {
            builder.ToTable("Medicines");

            builder.HasKey(m => m.MedicineId);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(m => m.Name)
                .IsUnique();

            builder.Property(m => m.GenericName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(m => m.Category)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(m => m.DefaultDosage)
                .HasMaxLength(50);

            builder.Property(m => m.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}