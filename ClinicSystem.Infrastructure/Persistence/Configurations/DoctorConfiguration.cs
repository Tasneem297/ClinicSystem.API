using ClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.Infrastructure.Persistence.Configurations;

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("Doctors");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.LastName).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Specialization).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Phone).IsRequired().HasMaxLength(20);
        builder.Property(d => d.Email).IsRequired().HasMaxLength(200);
        builder.Property(d => d.UserId).IsRequired().HasMaxLength(450);

        builder.HasIndex(d => d.UserId).IsUnique();
        builder.HasIndex(d => d.Email).IsUnique();
    }
}
