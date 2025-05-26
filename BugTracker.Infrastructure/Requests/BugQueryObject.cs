using BugTracker.Application.Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTracker.Infrastructure.Requests
{
    public class BugQueryObject : IQueryObject
    {
        public int Page { get; set; }
        public byte PageSize { get; set; }
        public string SortBy { get; set; } = "CreatedOn"; // Default sorting by CreatedDate in descending order
        public string Search { get; set; } = string.Empty; // Default to empty search, meaning no filtering by search term
    }
}
