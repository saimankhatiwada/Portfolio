using MediatR;
using Portfolio.Domain.Abstractions;

namespace Portfolio.Application.Abstractions.Messaging;


/// <summary>
/// Represents a command in the application, encapsulating the data and behavior required to perform an operation.
/// </summary>
/// <remarks>
/// This interface serves as a marker for commands that can be processed by command handlers.
/// It extends the <see cref="IRequest{TResponse}"/> interface from MediatR, ensuring compatibility with the MediatR pipeline.
/// Additionally, it inherits from <see cref="IBaseCommand"/>, providing a base abstraction for all commands.
/// </remarks>
public interface ICommand : IRequest<Result>, IBaseCommand
{
}

/// <summary>
/// Represents a command in the application that produces a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">The type of the response produced by the command.</typeparam>
/// <remarks>
/// This interface serves as a marker for commands that can be processed by command handlers.
/// It extends the <see cref="IRequest{TResponse}"/> interface from MediatR, ensuring compatibility with the MediatR pipeline.
/// Additionally, it inherits from <see cref="IBaseCommand"/>, providing a base abstraction for all commands.
/// </remarks>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}

/// <summary>
/// Serves as a base abstraction for all commands in the application.
/// </summary>
/// <remarks>
/// This interface is intended to be implemented by all command types, providing a common contract
/// for commands that can be processed by the application's command handling pipeline.
/// </remarks>
public interface IBaseCommand
{
}
