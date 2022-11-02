namespace ConcernsCaseWork.API.UseCases
{
    /// <summary>
    /// Represents a use case executor which accepts a request and returns a response. Enhances the standard IUseCase by supporting asynchronous execution.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IUseCaseAsync<in TRequest, TResponse>
    {
        Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken);
    }
}
