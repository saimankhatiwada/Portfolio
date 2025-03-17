using MediatR;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Messaging;

/// <summary>
/// Defines a handler for processing commands of type <typeparamref name="TCommand"/>.
/// </summary>
/// <typeparam name="TCommand">
/// The type of command to be handled. Must implement <see cref="ICommand"/>.
/// </typeparam>
/// <remarks>
/// This interface extends the <see cref="IRequestHandler{TRequest, TResponse}"/> interface from MediatR,
/// ensuring compatibility with the MediatR pipeline. It provides a mechanism to handle commands and return
/// a <see cref="Result"/> indicating the outcome of the operation.
/// </remarks>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}

/// <summary>
/// Defines a handler for processing commands of type <typeparamref name="TCommand"/> 
/// and producing a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TCommand">The type of the command to be handled.</typeparam>
/// <typeparam name="TResponse">The type of the response produced by the command handler.</typeparam>
/// <remarks>
/// This interface extends the <see cref="IRequestHandler{TRequest, TResponse}"/> interface from MediatR, 
/// ensuring compatibility with the MediatR pipeline. It is designed to handle commands that produce 
/// a result encapsulated in a <see cref="Result{TValue}"/> object, representing both success and failure states.
/// </remarks>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}
