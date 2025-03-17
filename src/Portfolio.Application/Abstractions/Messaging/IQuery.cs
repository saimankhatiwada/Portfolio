using MediatR;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Messaging;

/// <summary>
/// Represents a query that produces a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response produced by the query.
/// </typeparam>
/// <remarks>
/// This interface extends the <see cref="IRequest{TResponse}"/> interface, encapsulating the result 
/// of the query execution within a <see cref="Result{TResponse}"/> object. It is designed to be used 
/// as a marker interface for queries in the application.
/// </remarks>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
