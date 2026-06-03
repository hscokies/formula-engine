using Application.API.Formulas.Get;
using Microsoft.AspNetCore.Mvc;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Formulas;

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(GetFormulaQuery.Path, async (
                Guid id,
                GetFormulaHandler handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetFormulaQuery(id);
                var result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Formula)
            .Produces<NotFoundObjectResult>()
            .Produces<OkObjectResult>();
    }
}
