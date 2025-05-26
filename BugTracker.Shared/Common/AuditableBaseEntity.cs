namespace BugTracker.Shared.Common
{
    public class AuditableBaseEntity : BaseEntity, IAuditableEntity
    {
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; } = null!;
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public void SetAudit(string userId, bool isCreated)
        {
            if(isCreated)
            {
                CreatedOn = DateTime.UtcNow;
                CreatedBy = userId;
                LastModifiedBy = userId;
                LastModifiedOn = DateTime.UtcNow;
            }
            else
            {
                LastModifiedBy = userId;
                LastModifiedOn = DateTime.UtcNow;
            }
        }
    }
}
