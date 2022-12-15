using ConcernsCaseWork.API.Contracts.Enums;
using Newtonsoft.Json;

namespace ConcernsCaseWork.Service.Cases
{
	[Serializable]
	public sealed record CaseDto
	{
		public DateTimeOffset CreatedAt { get; init; }
		public DateTimeOffset UpdatedAt { get; init; }
		public DateTimeOffset ReviewAt { get; init; }
		public DateTimeOffset? ClosedAt { get; init; }
		public string CreatedBy { get; init; }
		public string Description { get; init; }
		public string CrmEnquiry { get; init; }
		public string TrustUkPrn { get; init; }
		public string ReasonAtReview { get; init; }
		public DateTimeOffset DeEscalation { get; init; }
		public string Issue { get; init; }
		public string CurrentStatus { get; init; }
		public string CaseAim { get; init; }
		public string DeEscalationPoint { get; init; }
		public string NextSteps { get; init; }
		
		/// <summary>
		/// Deteriorating, unchanged, improved
		/// </summary>
		public string DirectionOfTravel { get; init; }
		public long Urn { get; init; }
		public long StatusId { get; init; }
		public long RatingId { get; set; }
		public string CaseHistory { get; set; }
		public TerritoryEnum? Territory { get; set; }
	}
}