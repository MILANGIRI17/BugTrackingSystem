namespace BugTracker.Shared.Pagination
{
    public class PagedList<T>
    {
        public int TotalCount { get; set; }
        public IReadOnlyList<T>? Items { get; set; }
    }
}
