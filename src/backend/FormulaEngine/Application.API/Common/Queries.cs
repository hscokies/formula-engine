using Domain.Common;

namespace Application.API.Common;

public interface IQuery<TResponse>;

public interface IQueryHandler;

public interface IQueryHandler<in TQuery, TResponse> : IQueryHandler
    where TQuery : IQuery<TResponse>
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
