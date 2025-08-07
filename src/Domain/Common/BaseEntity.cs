namespace Domain.Common;

public abstract class BaseEntity : BaseEntity<Guid>
{
    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;

    private DateTime? _createdAt = DateTime.UtcNow;

    public DateTime? CreatedAt 
    { 
        get => _createdAt; 
        set => _createdAt = value ?? DateTime.UtcNow; 
    }
    
    public DateTime? LastModified { get; set;}

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }
}
