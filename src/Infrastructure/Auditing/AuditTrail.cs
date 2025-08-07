using System.Text.Json;

using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Auditing;

public class AuditTrail(EntityEntry entry)
{
    public EntityEntry Entry { get; } = entry;
    public string UserId { get; set; } = string.Empty;
    public string? TableName { get; set; }
    public Dictionary<string, object?> KeyValues { get; } = [];
    public Dictionary<string, object?> OldValues { get; } = [];
    public Dictionary<string, object?> NewValues { get; } = [];
    public List<PropertyEntry> TemporaryProperties { get; } = [];
    public AuditType AuditType { get; set; }
    public List<string> ChangedColumns { get; } = [];
    public bool HasTemporaryProperties => TemporaryProperties.Count > 0;

    public Audit ToAudit() => 
        new()
        {
            UserId = Guid.Parse(UserId),
            TableName = TableName,
            Type = AuditType.ToString(),
            DateTime = DateTime.UtcNow,
            PrimaryKey = JsonSerializer.Serialize(KeyValues),
            OldValues = OldValues.Count == 0 ? null : JsonSerializer.Serialize(OldValues),
            NewValues = NewValues.Count == 0 ? null : JsonSerializer.Serialize(NewValues),
            AffectedColumns = ChangedColumns.Count == 0 ? null : JsonSerializer.Serialize(ChangedColumns),
        };
}
