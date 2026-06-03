using Application.API.Formulas.Configure;
using Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Web.API.Infrastructure;
using Web.API.Requests.Formulas;

namespace Web.API.Endpoints.Formulas;

internal sealed class Configure : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(ConfigureFormulaCommand.Path, async (
                Guid id,
                ConfigureRequest request,
                ConfigureFormulaHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new ConfigureFormulaCommand(id, request.Name, request.Fields);
                var result = await handler.Handle(command, cancellationToken);

                return result.Match(() => Results.Ok(), CustomResults.Problem);
            })
            .RequireAuthorization(p => p.RequireRole(RoleName.Admin))
            .WithTags(Tags.Formula)
            .ProducesValidationProblem()
            .Produces<OkObjectResult>();
    }
}
