using Application.API.Formulas.Create;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Web.API.Infrastructure;
using Web.API.Requests.Formulas;

namespace Web.API.Endpoints.Formulas;

internal sealed class Create : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(CreateFormulaCommand.Path, async (
                HttpContext context,
                CreateRequest request,
                CreateFormulaHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateFormulaCommand(context.GetUserId(), request.Name, request.Expression);
                var result = await handler.Handle(command, cancellationToken);
                return result.Match(
                    (res) => Results.Created($"/api/formulas/{res.Id}", res),
                    CustomResults.Problem);
            })
            .RequireAuthorization(p => p.RequireRole(RoleName.Admin))
            .WithTags(Tags.Formula)
            .Produces<CreatedResult>();
    }
}
