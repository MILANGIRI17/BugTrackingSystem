using BugTracker.Shared.Enum;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Shared.Dtos
{
    public class BugCreateOrUpdateDto
    {
        public string? Id { get; set; }
        
        [Required]
        public string? TicketNumber { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? ReproductionStep { get; set; }

        public Status Status { get; set; }

        public Severity Severity { get; set; }

        public string? AssignToUserId { get; set; }

        public string? Assignee { get; set; }

        public List<AttachmentCreateOrUpdateDto> Attachments { get; set; } = new();

        public IFormFileCollection? FileAttachments { get; set; }
    }
}
