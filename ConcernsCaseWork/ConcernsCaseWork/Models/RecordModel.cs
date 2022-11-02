namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class RecordModel
	{
		public long Id { get; }
		
		public long CaseUrn { get; }
		
		public long TypeId { get; }
		
		public MeansOfReferralModel MeansOfReferralModel { get; }
		
		public TypeModel TypeModel { get; }

		public long RatingId { get; }
		
		public RatingModel RatingModel { get; }
		
		public long StatusId { get; }
		
		public StatusModel StatusModel { get; }
		
		public RecordModel(
			long caseUrn, 
			long typeId, 
			TypeModel typeModel, 
			long ratingId, 
			RatingModel ratingModel, 
			long id, 
			long statusId, 
			StatusModel statusModel, 
			MeansOfReferralModel meansOfReferralModel = null) => 
				(CaseUrn, TypeId, TypeModel, RatingId, RatingModel, Id, StatusId, StatusModel, MeansOfReferralModel) = 
				(caseUrn, typeId, typeModel, ratingId, ratingModel, id, statusId, statusModel, meansOfReferralModel);
	}
}