namespace Application.Core.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
