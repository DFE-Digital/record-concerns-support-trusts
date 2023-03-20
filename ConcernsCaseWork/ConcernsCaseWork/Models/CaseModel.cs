using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Service.Cases;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class CaseModel
	{
		public DateTimeOffset CreatedAt { get; set; }

		public DateTimeOffset UpdatedAt { get; set; }

		public DateTimeOffset ReviewAt { get; set; }

		public DateTimeOffset? ClosedAt { get; set; }
		
		/// <summary>
		/// Case owner from azure AD some unique identifier
		/// </summary>
		public string CreatedBy { get; set; }

		public string Description { get; set; }

		public string CrmEnquiry { get; set; }
		
		public string TrustUkPrn { get; set; }
		
		public string ReasonAtReview { get; set; }

		public DateTimeOffset DeEscalation { get; set; }

		public string Issue { get; set; } = string.Empty;

		public string CurrentStatus { get; set; } = string.Empty;

		public string CaseAim { get; set; } = string.Empty;
		
		public string DeEscalationPoint { get; set; } = string.Empty;
		
		public string NextSteps { get; set; } = string.Empty;

		/// <summary>
		/// Deteriorating, unchanged, improved
		/// </summary>
		public string DirectionOfTravel { get; set; } = DirectionOfTravelEnum.Deteriorating.ToString();

		public long Urn { get; set; }

		public long RatingId { get; set; }
		
		public RatingModel RatingModel { get; set; }
		
		public long StatusId { get; set; }
		
		public string StatusName { get; set; } = string.Empty;

		public IList<RecordModel> RecordsModel { get; set; } = new List<RecordModel>();
		
		public string PreviousUrl { get; set; }
		
		public string CaseHistory { get; set; }
		
		public Territory? Territory { get; set; }
		
		public bool ShowUpdateMessage { get; set; }
		
		public bool ShowValidationMessage { get; set; }
	}
}