using Ardalis.GuardClauses;

namespace ConcernsCaseWork.Logging;

public record CorrelationContext() : ICorrelationContext
{
	public string CorrelationId { get; private set; }
	public string CausationId { get; private set; }
	public string RequestId { get; private set; }
	public void SetContext(string correlationId, string causationId, string requestId)
	{
		this.CorrelationId = Guard.Against.NullOrWhiteSpace(correlationId);
		this.CausationId = Guard.Against.NullOrWhiteSpace(causationId);
		this.RequestId = Guard.Against.NullOrWhiteSpace(requestId);
	}
}