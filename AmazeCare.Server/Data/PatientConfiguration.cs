using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AmazeCare.Server.Models;
namespace AmazeCare.Server.Data
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("Patients");

            builder.HasKey(p => p.PatientId);

            builder.Property(p => p.PatientId)
                .UseIdentityColumn();

            builder.Property(p => p.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.DateOfBirth)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(p => p.Gender)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(p => p.PhoneNumber)
                   .IsRequired()
                   .HasMaxLength(15);

            builder.Property(p => p.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.UpdatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Ignore(p => p.Age);

            builder.HasOne(p => p.User)
                  .WithMany(u => u.Patients)
                  .HasForeignKey(p => p.UserId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);  


        }
    }
}
