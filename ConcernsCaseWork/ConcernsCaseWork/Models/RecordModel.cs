namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class RecordModel
	{
		public long Urn { get; }
		
		public long CaseUrn { get; }

		public long TypeUrn { get; }
		
		public TypeModel TypeModel { get; set; }

		public long RatingUrn { get; }
		
		public RatingModel RatingModel { get; set; }
		
		public long StatusUrn { get; }
		
		public RecordModel(long caseUrn, long typeUrn, long ratingUrn, long urn, long statusUrn) => 
			(CaseUrn, TypeUrn, RatingUrn, Urn, StatusUrn) = (caseUrn, typeUrn, ratingUrn, urn, statusUrn);
	}
}