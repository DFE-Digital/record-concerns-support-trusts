namespace ConcernsCaseWork.Data.Models
{
	public class NTIUnderConsiderationReasonMapping: IAuditable
	{
		public int Id { get; set; }

        public long NTIUnderConsiderationId { get; set; }
		public virtual NTIUnderConsideration NTIUnderConsideration { get; set; }

		public int NTIUnderConsiderationReasonId { get; set; }
		public virtual NTIUnderConsiderationReason NTIUnderConsiderationReason { get; set; }
	}
}
