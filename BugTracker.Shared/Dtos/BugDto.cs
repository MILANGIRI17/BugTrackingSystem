using BugTracker.Shared.Enum;

namespace BugTracker.Shared.Dtos
{
    public class BugDto
    {
        public string? Id { get; set; }
        public string? TicketNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? ReproductionStep { get; set; }
        public Status Status { get; set; }
        public Severity Severity { get; set; }
        public string? AssignToUserId { get; set; }
        public string? Assignee { get; set; }
        public List<string> FileNames { get; set; } = new();
        public string SearchValues => $"{TicketNumber} {Title} {Description}";
    }
}
