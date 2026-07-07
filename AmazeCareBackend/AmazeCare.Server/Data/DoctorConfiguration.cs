using AmazeCare.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AmazeCare.Server.Data.Configurations
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.ToTable("Doctors");

            builder.HasKey(d => d.DoctorId);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Specialization)
               .IsRequired()
               .HasMaxLength(100);

            builder.Property(d => d.Qualification)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(d => d.Designation)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.ExperienceYears)
                .IsRequired();

            builder.Property(d => d.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(d => d.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(d => d.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasIndex(d => d.IsActive)
                .HasDatabaseName("IX_Doctors_IsActive");

            builder.HasIndex(d => d.Specialization)
                .HasDatabaseName("IX_Doctors_Specialization");

            builder.HasIndex(d => d.Name)
                .HasDatabaseName("IX_Doctors_Name");

            // ← FK relationship to User
            builder.HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}