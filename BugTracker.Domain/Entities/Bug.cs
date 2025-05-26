using BugTracker.Shared.Common;
using BugTracker.Shared.Enum;

namespace BugTracker.Domain.Entities
{
    public class Bug : AuditableBaseEntity
    {
        public string? TicketNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ReproductionStep { get; set; }
        public Status Status { get; set; }
        public Severity Severity { get; set; }
        public string? AssignToUserId { get; set; }
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
