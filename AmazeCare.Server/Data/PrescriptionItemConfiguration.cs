using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AmazeCare.Server.Models;

namespace AmazeCare.Server.Data
{
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems");

            builder.HasKey(pi => pi.PrescriptionItemId);

            builder.Property(pi => pi.PrescriptionItemId)
                   .UseIdentityColumn();

            builder.Property(pi => pi.Morning)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(pi => pi.Afternoon)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(pi => pi.Evening)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(pi => pi.Night)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(pi => pi.FoodInstruction)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(20)
                   .HasDefaultValue(FoodInstruction.AfterFood)
                   .HasSentinel((FoodInstruction)(-1));

            builder.Property(pi => pi.Dosage)
                   .HasMaxLength(50);             

            builder.Property(pi => pi.DurationDays)
                   .IsRequired();

            builder.Property(pi => pi.Instructions)
                   .HasMaxLength(300);

            builder.HasOne(pi => pi.Prescription)
                   .WithMany(p => p.Items)
                   .HasForeignKey(pi => pi.PrescriptionId)
                   .OnDelete(DeleteBehavior.Cascade);  
       
            builder.HasOne(pi => pi.Medicine)
                   .WithMany(m => m.PrescriptionItems)
                   .HasForeignKey(pi => pi.MedicineId)
                   .OnDelete(DeleteBehavior.Restrict);  
        }
    }
}