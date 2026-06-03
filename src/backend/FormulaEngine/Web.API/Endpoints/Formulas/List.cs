using Application.API.Formulas.List;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Formulas;

internal sealed class List : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(ListFormulasQuery.Path, async (
            HttpContext context,
            [FromQuery] string? search,
            ListFormulasHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new ListFormulasQuery(context.GetUserId(), search);
            var result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .RequireAuthorization(x => x.RequireRole(RoleName.Admin))
        .WithTags(Tags.Formula)
        .Produces<OkObjectResult>();
    }
}
