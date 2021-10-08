using ConcernsCaseWork.Extensions;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class HomeModel
	{
		private const string DateFormat = "dd-MM-yyyy";
		
		public string CaseUrn { get; }

		public DateTimeOffset CreatedDateTimeOffset { get; }

		public string Created
		{
			get
			{
				return CreatedDateTimeOffset.ToString(DateFormat);
			}
		}

		public long CreatedUnixTime { get; }
		
		public DateTimeOffset UpdatedDateTimeOffset { get; }
		
		public string Updated 
		{ 
			get
			{
				return UpdatedDateTimeOffset.ToString(DateFormat);
			} 
		}
		
		public long UpdatedUnixTime { get; }
		
		public DateTimeOffset ClosedDateTimeOffset { get; }

		public string Closed
		{
			get
			{
				return ClosedDateTimeOffset.ToString(DateFormat);
			}
		}
		
		public DateTimeOffset ReviewDateTimeOffset { get; }

		public string Review
		{
			get
			{
				return ReviewDateTimeOffset.ToString(DateFormat);
			}
		}

		public string TrustNameTitle
		{
			get
			{
				return TrustName.ToTitle();
			}
		}
		
		public string TrustName { get; }
		
		public string AcademyNames { get; }
		
		public string CaseTypeDescription
		{
			get
			{
				var separator = string.IsNullOrEmpty(CaseSubType) ? string.Empty : ":";
				return $"{CaseType}{separator} {CaseSubType ?? string.Empty}";
			}
		}
		
		public string CaseType { get; }
		
		public string CaseSubType { get; }
		
		public IList<string> RagRating { get; }
		
		public IList<string> RagRatingCss { get; }
		
		public HomeModel(string caseUrn, DateTimeOffset created, DateTimeOffset updated, DateTimeOffset closed, DateTimeOffset review,
			string trustName, string academyNames, string caseType, string caseSubType, 
			IList<string> ragRating, IList<string> ragRatingCss) => 
			(CaseUrn, CreatedDateTimeOffset, UpdatedDateTimeOffset, ClosedDateTimeOffset, ReviewDateTimeOffset, TrustName, AcademyNames, CaseType, CaseSubType, RagRating, RagRatingCss) = 
			(caseUrn, created, updated, closed, review, trustName, academyNames, caseType, caseSubType, ragRating, ragRatingCss);
	}
}