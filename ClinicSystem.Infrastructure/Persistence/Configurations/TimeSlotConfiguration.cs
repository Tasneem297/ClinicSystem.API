using ClinicSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClinicSystem.Infrastructure.Persistence.Configurations;

public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TimeSlots");

        builder.HasKey(ts => ts.Id);

        builder.HasOne(ts => ts.Doctor)
            .WithMany(d => d.TimeSlots)
            .HasForeignKey(ts => ts.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ts => new { ts.DoctorId, ts.Date });
    }
}
