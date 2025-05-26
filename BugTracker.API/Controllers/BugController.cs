using BugTracker.Application.Logger;
using BugTracker.Application.Repositories;
using BugTracker.Application.Services;
using BugTracker.Domain.Entities;
using BugTracker.Infrastructure.Identity;
using BugTracker.Infrastructure.Requests;
using BugTracker.Shared.Constants;
using BugTracker.Shared.Dtos;
using BugTracker.Shared.Enum;
using BugTracker.Shared.Helper;
using BugTracker.Shared.Pagination;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace BugTracker.API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class BugController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private ICurrentUserService _currentUserService;
        private IExceptionLogger _exceptionLogger;
        public BugController(IUnitOfWork uow, ICurrentUserService currentUserService, IExceptionLogger exceptionLogger)
        {
            _uow = uow;
            _currentUserService = currentUserService;
            _exceptionLogger = exceptionLogger;
        }

        [HttpGet("get-bug/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            string[] includingProperties = new string[] { "Attachments" };
            var bug = await _uow.Repo<Bug>().GetAsync(x => x.Id == id, includingProperties);
            var bugDto = bug.Adapt<BugDto>();
            foreach (var attachment in bug.Attachments)
            {
                bugDto.FileNames.Add(attachment.Path);
            }
            return Ok(ResponseHandler<BugDto>.SuccessResopnse(bugDto));
        }

        [HttpGet("get-bugs")]
        public async Task<IActionResult> GetAllBugs()
        {
            try
            {
                var filter = getBugUserFilter(string.Empty);
                if (filter == null)
                    return BadRequest(ResponseHandler<string>.FailureResopnse("User Not Found"));

                var bugs = await _uow.Repo<Bug>().GetAllAsync(filter);
                return Ok(ResponseHandler<List<BugDto>>.SuccessResopnse(bugs.Adapt<List<BugDto>>()));
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return BadRequest(ResponseHandler<string>.FailureResopnse("Exception occurred"));
            }
        }

        [HttpGet("get-paged-bugs")]
        public async Task<IActionResult> GetPagedBugs([FromQuery] BugQueryObject? queryObject = null)
        {
            try
            {
                var filter = getBugUserFilter(queryObject.Search);
                if (filter == null)
                    return BadRequest(ResponseHandler<string>.FailureResopnse("User Not Found"));

                var bugs = await _uow.Repo<Bug>().GetPagedAsync(queryObject, filter);
                return Ok(ResponseHandler<PagedList<BugDto>>.SuccessResopnse(bugs.Adapt<PagedList<BugDto>>()));
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                return BadRequest(ResponseHandler<string>.FailureResopnse("Exception occurred"));
            }
        }

        [Authorize(Roles = DefaultUserRole.Developer)]
        [HttpGet("search-bugs/{searchWord}")]
        public async Task<IActionResult> SearchBugs(string searchWord)
        {
            var filter = getBugUserFilter(searchWord);
            if (filter == null)
                return BadRequest(ResponseHandler<string>.FailureResopnse("User Not Found"));
            var bugs = await _uow.Repo<Bug>().GetAllAsync();

            return Ok(ResponseHandler<List<BugDto>>.SuccessResopnse(bugs.Adapt<List<BugDto>>()));
        }

        [HttpGet("get-developers")]
        public async Task<IActionResult> GetDevelopers()
        {
            var users = await _uow.Repo<User>().GetAllAsync(x => x.IsDeveloper);
            return Ok(ResponseHandler<List<UserDto>>.SuccessResopnse(users.Adapt<List<UserDto>>()));
        }

        [HttpPost("add-bug")]
        public async Task<ActionResult> Create([FromBody] BugCreateOrUpdateDto bugReportDto)
        {
            try
            {
                var bug = bugReportDto.Adapt<Bug>();
                if (bugReportDto.Attachments != null && bugReportDto.Attachments.Count > 0)
                {
                    foreach (var attachment in bugReportDto.Attachments)
                    {
                        bug.Attachments.Add(new Attachment
                        {
                            BugId = bug.Id,
                            Bug = bug,
                            Name = attachment.Name,
                            Path = attachment.Path
                        });
                    }
                }
                await _uow.Repo<Bug>().AddAsync(bug);
                await _uow.Commit();
                return Ok(ResponseHandler<string>.SuccessResopnse("Bug Created Successfully"));
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                await _uow.Rollback();
                return BadRequest(ResponseHandler<string>.FailureResopnse("Exception occurred"));
            }
        }

        [HttpPut("update-bug/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BugCreateOrUpdateDto bugDto)
        {
            try
            {
                var bug = await _uow.Repo<Bug>().GetByIdAsync(id);
                if (bug == null) return BadRequest(ResponseHandler<string>.FailureResopnse("Bug Not Found"));
                //bug.TicketNumber = bugReportDto.TicketNumber;
                //bug.Title = bugReportDto.Title;
                //bug.Description = bugReportDto.Description;
                //bug.ReproductionStep = bugReportDto.ReproductionStep;
                //bug.Status = bugReportDto.Status;
                //bug.Severity = bugReportDto.Severity;
                //bug.AssignToUserId = bugReportDto.AssignToUserId;
                //await _uow.Repo<Bug>().UpdateAsync(bug);
                bugDto.Adapt(bug);
                await _uow.Commit();
                return Ok((ResponseHandler<string>.SuccessResopnse("Bug Updated Successfully")));
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                await _uow.Rollback();
                return BadRequest(ResponseHandler<string>.FailureResopnse("Exception occurred"));
            }
        }

        [HttpDelete("delete-bug/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var bug = await _uow.Repo<Bug>().GetByIdAsync(id);
                if (bug == null) return BadRequest(ResponseHandler<string>.FailureResopnse("Bug Not Found"));
                await _uow.Repo<Bug>().DeleteAsync(bug);
                await _uow.Commit();
                return Ok((ResponseHandler<string>.SuccessResopnse("Bug Updated Successfully")));
            }
            catch (Exception ex)
            {
                _exceptionLogger.LogException(ex);
                await _uow.Rollback();
                return BadRequest(ResponseHandler<string>.FailureResopnse("Exception occurred"));
            }
        }

        private Expression<Func<Bug, bool>> getBugUserFilter(string? searchText)
        {
            Expression<Func<Bug, bool>> bugFilter = null;
            var userRoles = _currentUserService.GetUserRoles();
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userRoles) || string.IsNullOrEmpty(userId))
                return bugFilter;

            if (userRoles.Contains(DefaultUserRole.User))
                bugFilter = bug => bug.CreatedBy == userId;

            if (userRoles.Contains(DefaultUserRole.Developer))
            {
                bugFilter = bug => bug.AssignToUserId == userId || bug.Status == Status.Open;
                if (!string.IsNullOrEmpty(searchText))
                {
                    var lowerSearchText = searchText.ToLower();
                    bugFilter = bug => (bug.AssignToUserId == userId || bug.Status == Status.Open) &&
                                (bug.Title.ToLower().Contains(lowerSearchText) || bug.Description.ToLower().Contains(lowerSearchText) ||
                                 bug.TicketNumber.ToLower().Contains(lowerSearchText));
                }

            }
            return bugFilter;
        }
    }
}
