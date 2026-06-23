using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AmazeCare.Server.Models;

namespace AmazeCare.Server.Data
{
    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");

            builder.HasKey(p => p.PrescriptionId);

            builder.Property(p => p.PrescriptionId)
                   .UseIdentityColumn();

            builder.Property(p => p.Notes)
                   .HasMaxLength(2000);

            builder.Property(p => p.PrescribedDate)
                   .IsRequired()
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");
            
            builder.HasIndex(p => p.ConsultationId)
                   .IsUnique()
                   .HasDatabaseName("IX_Prescriptions_ConsultationId");
            
            builder.HasOne(p => p.Consultation)
                   .WithOne(c => c.Prescription)
                   .HasForeignKey<Prescription>(p => p.ConsultationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Patient)
                   .WithMany(p => p.Prescriptions)
                   .HasForeignKey(p => p.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Doctor)
                   .WithMany()
                   .HasForeignKey(p => p.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

            
        }
    }
}