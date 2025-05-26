using System.ComponentModel.DataAnnotations;

namespace BugTracker.Shared.Enum
{
    public enum Status
    {
        [Display(Name = "Open")]
        Open,

        [Display(Name = "In Progress")]
        InProgress,

        [Display(Name = "Resolved")]
        Resolved,

        [Display(Name = "Closed")]
        Closed
    }
}
