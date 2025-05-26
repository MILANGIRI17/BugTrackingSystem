using BugTracker.Shared.Common;

namespace BugTracker.Domain.Entities
{
    public class SystemExceptionLog : AuditableBaseEntity
    {
        public string? Log { get; set; }
    }
}
