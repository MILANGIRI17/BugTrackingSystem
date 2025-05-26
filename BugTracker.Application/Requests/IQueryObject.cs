namespace BugTracker.Application.Requests;

public interface IQueryObject
{
    int Page { get; set; }
    byte PageSize { get; set; }
    string SortBy { get; set; }
    string Search { get; set; }
}
