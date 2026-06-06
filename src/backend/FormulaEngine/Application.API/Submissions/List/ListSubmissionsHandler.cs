using Application.API.Common;
using Domain.Common;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Application.API.Submissions.List;

public sealed class ListSubmissionsHandler(IReadOnlyDataContext dataContext)
    : IQueryHandler<ListSubmissionsQuery, ListSubmissionsResult>
{
    public async Task<Result<ListSubmissionsResult>> Handle(
        ListSubmissionsQuery query,
        CancellationToken cancellationToken)
    {
        var submission = await dataContext.Submissions
            .Where(x => x.FormulaId == query.FormulaId)
            .OrderByDescending(x => x.Id)
            .Select(x => new SubmissionItem(x.Id, x.Result, x.UserId))
            .ToListAsync(cancellationToken);


        return new ListSubmissionsResult(submission);
    }
}
