namespace Courier;

public interface IQueryHandler<TResponse>
{
    public Task<TResponse> HandleAsync(CancellationToken ctx);
}

public interface IQueryHandler<in T, TResponse>
{
    public Task<TResponse> HandleAsync(T id, CancellationToken ctx);
}