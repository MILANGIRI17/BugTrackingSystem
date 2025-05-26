namespace BugTracker.Shared.Common
{
    public interface IAuditableEntity
    {
        DateTime CreatedOn { get; set; }
        string CreatedBy { get; set; }
        string LastModifiedBy { get; set; }
        DateTime? LastModifiedOn { get; set; }
        void SetAudit(string userId,bool isCreated);
    }
}
