using Infrastructure.Auditing;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class AuditLogsConfiguration : IEntityTypeConfiguration<Audit>
{
    public void Configure(EntityTypeBuilder<Audit> builder)
    {
        builder
            .Property(x => x.NewValues)
            .HasColumnType("TEXT");

        builder
            .Property(x => x.OldValues)
            .HasColumnType("TEXT");

        builder
            .Property(x => x.AffectedColumns)
            .HasColumnType("TEXT");

        builder
            .Property(x => x.PrimaryKey)
            .HasColumnType("TEXT");
    }
}
