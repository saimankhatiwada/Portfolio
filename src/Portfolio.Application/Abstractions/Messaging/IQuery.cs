using MediatR;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
