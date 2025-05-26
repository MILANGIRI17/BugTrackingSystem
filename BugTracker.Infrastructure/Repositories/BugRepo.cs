using BugTracker.Application.Repositories;
using BugTracker.Domain.Entities;
using BugTracker.Infrastructure.Data;
namespace BugTracker.Infrastructure.Repositories
{
    public class BugRepo : IBugRepo
    {
        private readonly AppDbContext _context;
        public BugRepo(AppDbContext context)
        {
            _context = context;
        }
    }
}
