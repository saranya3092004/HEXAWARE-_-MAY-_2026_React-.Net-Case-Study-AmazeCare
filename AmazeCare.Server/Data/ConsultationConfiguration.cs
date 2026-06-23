using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AmazeCare.Server.Models;

namespace AmazeCare.Server.Data
{
    public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
    {
        public void Configure(EntityTypeBuilder<Consultation> builder)
        {
            builder.ToTable("Consultations");

            builder.HasKey(c => c.ConsultationId);

            builder.Property(c => c.ConsultationId)
                   .UseIdentityColumn();

           
            builder.Property(c => c.CurrentSymptoms)
                   .IsRequired()
                   .HasMaxLength(1000);

           
            builder.Property(c => c.PhysicalExamination)
                   .HasMaxLength(1000);

            
            builder.Property(c => c.TreatmentPlan)
                   .HasMaxLength(1000);

            builder.Property(c => c.Diagnosis)
                   .HasMaxLength(500);

            builder.Property(c => c.ConsultationDate)
                   .IsRequired()
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");


            builder.HasIndex(c => c.AppointmentId)
                   .IsUnique()
                   .HasDatabaseName("IX_Consultations_AppointmentId");

          
            builder.HasOne(c => c.Appointment)
                   .WithOne(a => a.Consultation)
                   .HasForeignKey<Consultation>(c => c.AppointmentId)
                   .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasOne(c => c.Patient)
                   .WithMany(p => p.Consultations)
                   .HasForeignKey(c => c.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            
            builder.HasOne(c => c.Doctor)
                   .WithMany(d => d.Consultations)
                   .HasForeignKey(c => c.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);

           
        }
    }
}