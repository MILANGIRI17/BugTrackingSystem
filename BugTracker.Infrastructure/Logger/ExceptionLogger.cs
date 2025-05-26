using BugTracker.Application.Logger;
using BugTracker.Domain.Entities;
using BugTracker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Infrastructure.Logger
{
    public class ExceptionLogger : IExceptionLogger
    {
        private readonly AppDbContext _context;
        public ExceptionLogger(AppDbContext context)
        {
            _context = context;
        }

        public void LogException(Exception exception)
        {
            SystemExceptionLog systemLog = new SystemExceptionLog { Log = exception.ToString()};
            _context.Add(systemLog);
            _context.SaveChanges();
        }
    }
}
