using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AmazeCare.Server.Models;

namespace AmazeCare.Server.Data
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.AppointmentId);

            builder.Property(a => a.AppointmentId)
                   .UseIdentityColumn();

            builder.Property(a => a.AppointmentDate)
                   .IsRequired()
                   .HasColumnType("datetime2");

            builder.Property(a => a.TimeSlot)
                   .IsRequired()
                   .HasMaxLength(20);          // e.g. "10:00-10:30"

            builder.Property(a => a.Reason)
                   .HasMaxLength(500);

            builder.Property(a => a.VisitType)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(30)
                   .HasDefaultValue(VisitType.GeneralCheckup);

            builder.Property(a => a.Status)
                   .HasConversion<string>()
                   .IsRequired()
                   .HasMaxLength(20)
                   .HasDefaultValue(AppointmentStatus.Pending);

            builder.Property(a => a.CancellationReason)
                   .HasMaxLength(500);

            builder.Property(a => a.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(a => a.UpdatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.TimeSlot })
                   .IsUnique()
                   .HasFilter("[Status] <> 'Cancelled' AND [Status] <> 'Rejected' AND [Status] <> 'NoShow'")
                   .HasDatabaseName("IX_Appointments_Doctor_Date_TimeSlot_Unique");

            builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate })
                   .HasDatabaseName("IX_Appointments_Doctor_Date");

            builder.HasIndex(a => new { a.PatientId, a.AppointmentDate })
                   .HasDatabaseName("IX_Appointments_Patient_Date");


            builder.HasOne(a => a.Patient)
                   .WithMany(p => p.Appointments)
                   .HasForeignKey(a => a.PatientId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(a => a.Doctor)
                   .WithMany(d => d.Appointments)
                   .HasForeignKey(a => a.DoctorId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}