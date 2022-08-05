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
		
		public MeansOfReferralModel MeansOfReferralModel { get; }
		
		public TypeModel TypeModel { get; }

		public long RatingUrn { get; }
		
		public RatingModel RatingModel { get; }
		
		public long StatusUrn { get; }
		
		public StatusModel StatusModel { get; }
		
		public RecordModel(
			long caseUrn, 
			long typeUrn, 
			TypeModel typeModel, 
			long ratingUrn, 
			RatingModel ratingModel, 
			long urn, 
			long statusUrn, 
			StatusModel statusModel, 
			MeansOfReferralModel meansOfReferralModel = null) => 
				(CaseUrn, TypeUrn, TypeModel, RatingUrn, RatingModel, Urn, StatusUrn, StatusModel, MeansOfReferralModel) = 
				(caseUrn, typeUrn, typeModel, ratingUrn, ratingModel, urn, statusUrn, statusModel, meansOfReferralModel);
	}
}