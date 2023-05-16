namespace Courier;

public interface ICommandHandler<in TRequest, TResponse>
{
    public Task<TResponse> HandleAsync(TRequest request, CancellationToken ctx);
}

public interface ICommandHandler<in T, in TRequest, TResponse>
{
    public Task<TResponse> HandleAsync(T id, TRequest request, CancellationToken ctx);
}