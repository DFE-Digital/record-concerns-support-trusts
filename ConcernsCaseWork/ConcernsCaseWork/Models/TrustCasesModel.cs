using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using static ConcernsCaseWork.Extensions.DateExtension;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TrustCasesModel
	{
		public long CaseUrn { get; }
		
		public Tuple<int, IList<string>> RagRating { get; }
		
		public IList<string> RagRatingCss { get; }
		
		/// <summary>
		/// Case DB entity doesn't allow null values for the dates.
		/// When a case is created all dates are the current date at that point,
		/// to be able to identify that a case was closed compare the createdAt
		/// with close date, if different use the close date for display. 
		/// </summary>
		public string Closed 
		{ 
			get
			{
				var sameDate = CreatedAtDateTimeOffset.CompareTo(ClosedDateTimeOffset) == 0;
				return sameDate ? "-" : ClosedDateTimeOffset.ToDayMonthYear();
			} 
		}
		
		public string CaseTypeDescription
		{
			get
			{
				var separator = string.IsNullOrEmpty(CaseSubType) ? string.Empty : ":";
				return $"{CaseType}{separator} {CaseSubType ?? string.Empty}";
			}
		}

		public string StatusDescription
		{
			get
			{
				if (Status.Equals(StatusEnum.Live.ToString(), StringComparison.OrdinalIgnoreCase))
				{
					return "Open";
				}
				return Status.Equals(StatusEnum.Close.ToString(), StringComparison.OrdinalIgnoreCase) ? "Closed" : "-";
			}
		}
		
		private string CaseType { get; }
		
		private string CaseSubType { get; }
		
		private DateTimeOffset CreatedAtDateTimeOffset { get; }
		
		private DateTimeOffset ClosedDateTimeOffset { get; }
		
		private string Status { get; }
		
		public TrustCasesModel(long caseUrn, string caseType, string caseSubType, Tuple<int, IList<string>> ragRating, IList<string> ragRatingCss, DateTimeOffset createdAt, DateTimeOffset closed, string status) => 
			(CaseUrn, CaseType, CaseSubType, RagRating, RagRatingCss, CreatedAtDateTimeOffset, ClosedDateTimeOffset, Status) = 
			(caseUrn, caseType, caseSubType, ragRating, ragRatingCss, createdAt, closed, status);
	}
}