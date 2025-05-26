namespace BugTracker.Shared.Dtos
{
    public class AttachmentCreateOrUpdateDto
    {
        public string? BugId { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
    }
}
