using BugTracker.Shared.Common;

namespace BugTracker.Domain.Entities
{
    public class Attachment : BaseEntity
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
        public string BugId { get; set; } = null!;
        public Bug Bug { get; set; }
    }
}
