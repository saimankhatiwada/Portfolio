using MediatR;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand
{
}
public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}
public interface IBaseCommand
{
}
