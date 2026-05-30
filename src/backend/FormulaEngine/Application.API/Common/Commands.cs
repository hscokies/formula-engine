using Domain.Common;

namespace Application.API.Common;

internal interface ICommand;
internal interface ICommand<TResponse> : ICommand;

internal interface ICommandHandler;
internal interface ICommandHandler<in TCommand> : ICommandHandler
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
}

internal interface ICommandHandler<in TCommand, TResponse> : ICommandHandler
    where TCommand : ICommand<TResponse>
{
    Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}
