using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AmazeCare.Server.Models;

namespace AmazeCare.Server.Data
{
    public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
    {
        public void Configure(EntityTypeBuilder<Specialization> builder)
        {
            builder.ToTable("Specializations");

            builder.HasKey(s => s.SpecializationId);

            builder.Property(s => s.SpecializationId)
                   .UseIdentityColumn();

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(s => s.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.HasIndex(s => s.Name)
                   .IsUnique()
                   .HasDatabaseName("IX_Specializations_Name");

           
            builder.HasData(
                new Specialization { SpecializationId = 1, Name = "General Medicine", IsActive = true },
                new Specialization { SpecializationId = 2, Name = "Obstetrics and Gynecology",  IsActive = true },
                new Specialization { SpecializationId = 3, Name = "Pediatrics",  IsActive = true },
                new Specialization { SpecializationId = 4, Name = "Cardiology",  IsActive = true },
                new Specialization { SpecializationId = 5, Name = "Orthopedics",  IsActive = true },
                new Specialization { SpecializationId = 6, Name = "Dermatology",  IsActive = true },
                new Specialization { SpecializationId = 7, Name = "Neurology",  IsActive = true },
                new Specialization { SpecializationId = 8, Name = "Ophthalmology",  IsActive = true },
                new Specialization { SpecializationId = 9, Name = "ENT", IsActive = true },
                new Specialization { SpecializationId = 10, Name = "Psychiatry",  IsActive = true },
                new Specialization { SpecializationId = 11, Name = "Urology",  IsActive = true },
                new Specialization { SpecializationId = 12, Name = "Gastroenterology", IsActive = true },
                new Specialization { SpecializationId = 13, Name = "Pulmonology",  IsActive = true },
                new Specialization { SpecializationId = 14, Name = "Endocrinology",  IsActive = true },
                new Specialization { SpecializationId = 15, Name = "Oncology", IsActive = true },
                new Specialization { SpecializationId = 16, Name = "Nephrology",  IsActive = true },
                new Specialization { SpecializationId = 17, Name = "Rheumatology",  IsActive = true },
                new Specialization { SpecializationId = 18, Name = "Emergency Medicine",IsActive = true }
            );
        }
    }
}