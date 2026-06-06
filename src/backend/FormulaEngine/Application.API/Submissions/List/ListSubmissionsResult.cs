namespace Application.API.Submissions.List;

public sealed record ListSubmissionsResult(IEnumerable<SubmissionItem> Items);

public sealed record SubmissionItem(Guid Id, decimal Result, Guid UserId);
