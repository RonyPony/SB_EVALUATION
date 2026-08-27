namespace SB.BACKEND.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public bool IsDeleted { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public Guid? DeletedBy { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public void ApplyCreatedAudit(DateTimeOffset now, Guid? userId)
    {
        CreatedAt = now.ToUniversalTime();
        CreatedBy = userId;
        IsActive = true;
        IsDeleted = false;
    }

    public void ApplyUpdatedAudit(DateTimeOffset now, Guid? userId)
    {
        UpdatedAt = now.ToUniversalTime();
        UpdatedBy = userId;
    }

    protected void SoftDelete(DateTimeOffset now, Guid? userId)
    {
        IsDeleted = true;
        IsActive = false;
        DeletedAt = now.ToUniversalTime();
        DeletedBy = userId;
        ApplyUpdatedAudit(now, userId);
    }

    protected void Restore(DateTimeOffset now, Guid? userId)
    {
        IsDeleted = false;
        IsActive = true;
        DeletedAt = null;
        DeletedBy = null;
        ApplyUpdatedAudit(now, userId);
    }
}
