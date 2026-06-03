using Application.API.Formulas.Submit;
using Domain.Expressions;
using Microsoft.AspNetCore.Mvc;
using Web.API.Infrastructure;

namespace Web.API.Endpoints.Formulas;

internal sealed class Submit : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(SubmitEvaluationCommand.Path, async (
                HttpContext context,
                Guid formulaId,
                Guid evaluationId,
                Dictionary<string, IValue> arguments,
                SubmitEvaluationHandler handler,
                CancellationToken cancellationToken) =>
            {
                var command = new SubmitEvaluationCommand(context.GetUserId(),formulaId, evaluationId, arguments);
                var result = await handler.Handle(command, cancellationToken);
                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .RequireAuthorization()
            .WithTags(Tags.Formula)
            .ProducesValidationProblem()
            .Produces<NoContentResult>();
    }
}
