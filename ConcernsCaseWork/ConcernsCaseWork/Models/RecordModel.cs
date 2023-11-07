using ConcernsCaseWork.API.Contracts.Concerns;

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
		
		public long StatusId { get; set; }

		public RecordModel()
		{ 
		}
		
		public RecordModel(
			long caseUrn, 
			long typeId, 
			TypeModel typeModel, 
			long ratingId, 
			long id, 
			long statusId, 
			MeansOfReferralModel meansOfReferralModel = null) => 
				(CaseUrn, TypeId, TypeModel, RatingId, Id, StatusId, MeansOfReferralModel) = 
				(caseUrn, typeId, typeModel, ratingId, id, statusId, meansOfReferralModel);

		public bool IsClosed()
		{
			return StatusId == (long) ConcernStatus.Close;
		}
	}
}