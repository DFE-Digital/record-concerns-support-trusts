using Service.TRAMS.Cases;
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

		public DateTimeOffset ClosedAt { get; set; }
		
		/// <summary>
		/// Case owner from azure AD some unique identifier
		/// </summary>
		public string CreatedBy { get; set; }

		public string Description { get; set; }

		public string CrmEnquiry { get; set; }
		
		public string TrustName { get; set; }
		
		public string TrustUkPrn { get; set; }
		
		public string ReasonAtReview { get; set; }

		public DateTimeOffset DeEscalation { get; set; }

		public string Issue { get; set; }

		public string CurrentStatus { get; set; }

		public string CaseAim { get; set; }
		
		public string DeEscalationPoint { get; set; }
		
		public string NextSteps { get; set; }

		/// <summary>
		/// Deteriorating, unchanged, improved
		/// </summary>
		public string DirectionOfTravel { get; set; } = DirectionOfTravelEnum.Deteriorating.ToString();

		public long Urn { get; set; }

		public long Status { get; set; }
		
		public string StatusName { get; set; } = string.Empty;

		public string CaseTypeDescription
		{
			get
			{
				return string.IsNullOrEmpty(CaseSubType) ? CaseType : CaseSubType;
			}
		}
		
		public string CaseType { get; set; } = string.Empty;

		public string CaseSubType { get; set; } = string.Empty;

		public string RagRatingName { get; set; } = string.Empty;
		
		public IList<string> RagRating { get; set; }

		public IList<string> RagRatingCss { get; set; }
		
		public IDictionary<string, IList<string>> TypesDictionary { get; set; }
		
		public TrustDetailsModel TrustDetailsModel { get; set; }
		
		public string PreviousUrl { get; set; }
	}
}