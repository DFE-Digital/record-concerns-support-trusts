namespace ConcernsCaseWork.Logging
{
	public interface ICorrelationContext
	{
		public string CorrelationId { get; }
		public string CausationId { get; }
		public string RequestId { get; }
		public void SetContext(string correlationId, string causationId, string requestId);
	}
}
