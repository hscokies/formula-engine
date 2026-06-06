using Application.API.Submissions.List;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Submissions;

internal sealed class ListSubmissions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ListSubmissionsQuery.Path, async (
                Guid id,
                ListSubmissionsHandler handler,
                CancellationToken cancellationToken) =>
            {
                var query = new ListSubmissionsQuery(id);
                var result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization(x => x.RequireRole(RoleName.Admin))
            .WithTags(Tags.Formula)
            .Produces<OkObjectResult>();
    }
}
