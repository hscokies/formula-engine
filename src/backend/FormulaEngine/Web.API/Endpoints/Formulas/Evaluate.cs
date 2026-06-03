using Application.API.Formulas.Evaluate;
using Domain.Expressions;
using Microsoft.AspNetCore.Mvc;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Formulas;

internal sealed class Evaluate : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(EvaluateFormulaCommand.Path, async (
                Guid id,
                Dictionary<string, IValue> arguments,
                EvaluateFormulaHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new EvaluateFormulaCommand(id, arguments);
                var result = await handler.Handle(command, cancellationToken);
                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Formula)
            .ProducesValidationProblem()
            .Produces<OkObjectResult>();
    }
}
