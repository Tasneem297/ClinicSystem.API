using ClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Phone).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(200);
        builder.Property(p => p.UserId).IsRequired().HasMaxLength(450);

        builder.HasIndex(p => p.UserId).IsUnique();
        builder.HasIndex(p => p.Email).IsUnique();
    }
}
