using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Records
{
	public sealed class CreateRecordDto
	{
		[JsonProperty("createdAt")]
		public DateTimeOffset CreatedAt { get; }

		[JsonProperty("updatedAt")]
		public DateTimeOffset UpdatedAt { get; }
		
		[JsonProperty("reviewAt")]
		public DateTimeOffset ReviewAt { get; }

		[JsonProperty("name")]
		public string Name { get; }
		
		[JsonProperty("description")]
		public string Description { get; }
		
		[JsonProperty("reason")]
		public string Reason { get; }
		
		[JsonProperty("caseUrn")]
		public long CaseUrn { get; }
		
		[JsonProperty("typeId")]
		public long TypeId { get; }

		[JsonProperty("ratingId")]
		public long RatingId { get; }

		[JsonProperty("ratingRational")]
		public bool RatingRational { get; }

		[JsonProperty("ratingRationalCommentary")]
		public string RatingRationalCommentary { get; }

		[JsonProperty("statusId")]
		public long StatusId { get; }
		
		[JsonProperty("meansOfReferralId")]
		public long MeansOfReferralId { get; }
		
		[JsonConstructor]
		public CreateRecordDto(DateTimeOffset createdAt, DateTimeOffset updatedAt, DateTimeOffset reviewAt, 
			string name, string description, string reason, long caseUrn, long typeId, 
			long ratingId, bool ratingRational, string ratingRationalCommentary, long statusId, long meansOfReferralId) => 
			(CreatedAt, UpdatedAt, ReviewAt, Name, Description, Reason, CaseUrn, TypeId, RatingId, RatingRational, RatingRationalCommentary, StatusId, MeansOfReferralId) = 
			(createdAt, updatedAt, reviewAt, name, description, reason, caseUrn, typeId, ratingId, ratingRational, ratingRationalCommentary, statusId, meansOfReferralId);
	}
}