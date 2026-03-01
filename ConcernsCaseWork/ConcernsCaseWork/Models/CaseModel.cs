using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Service.Cases;
using System;
using System.Collections.Generic;
using System.Linq;

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

		/// <summary>
		/// Team leader from azure AD some unique identifier
		/// </summary>
		public string TeamLedBy { get; set; }

		public string Description { get; set; }

		public string CrmEnquiry { get; set; }

		public string TrustUkPrn { get; set; }

		public string ReasonAtReview { get; set; }

		public DateTimeOffset? DeEscalation { get; set; }

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

		public bool RatingRational { get; init; }
		public string RatingRationalCommentary { get; init; }

		public long StatusId { get; set; }

		public IList<RecordModel> RecordsModel { get; set; } = new List<RecordModel>();

		public string PreviousUrl { get; set; }

		public string CaseHistory { get; set; }

		public Territory? Territory { get; set; }

		public Division? Division { get; set; }

		public Region? Region { get; set; }

		public bool ShowUpdateMessage { get; set; }

		public bool ShowValidationMessage { get; set; }

		public bool IsArchived { get;set; }

		public bool IsConcernsCase()
		{
			return RecordsModel.Any();
		}

		public bool IsOpen()
		{
			return StatusId == (int)CaseStatus.Live;
		}

		public bool IsClosed()
		{
			return StatusId == (int)CaseStatus.Close;
		}

		/// <summary>
		/// This differs depending on whether the case is for regions group or SFSO
		/// SFSO - Territory
		/// Regions Group - Region
		/// </summary>
		public string Area { get; set; }
	}
}