using MediatR;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Messaging;

/// <summary>
/// Defines a handler for processing queries of type <typeparamref name="TQuery"/> and producing a result of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">
/// The type of the query to be handled. It must implement the <see cref="IQuery{TResponse}"/> interface.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response produced by the query handler.
/// </typeparam>
/// <remarks>
/// This interface extends the <see cref="IRequestHandler{TRequest, TResponse}"/> interface, where the request is a query of type <typeparamref name="TQuery"/> 
/// and the response is encapsulated within a <see cref="Result{TResponse}"/> object. It is designed to be implemented by classes responsible for handling 
/// specific queries in the application.
/// </remarks>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
