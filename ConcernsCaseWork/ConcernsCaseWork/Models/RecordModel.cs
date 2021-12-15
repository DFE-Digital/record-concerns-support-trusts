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
		
		public TypeModel TypeModel { get; }

		public long RatingUrn { get; }
		
		public RatingModel RatingModel { get; }
		
		public long StatusUrn { get; }
		
		public RecordModel(long caseUrn, long typeUrn, TypeModel typeModel, long ratingUrn, RatingModel ratingModel, long urn, long statusUrn) => 
			(CaseUrn, TypeUrn, TypeModel, RatingUrn, RatingModel, Urn, StatusUrn) = 
			(caseUrn, typeUrn, typeModel, ratingUrn, ratingModel, urn, statusUrn);
	}
}