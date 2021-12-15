using ConcernsCaseWork.Extensions;
using Service.Redis.Models;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class HomeModel
	{
		public string CaseUrn { get; }

		public string CreatedBy { get; }
		
		private DateTimeOffset CreatedDateTimeOffset { get; }

		public string Created
		{
			get
			{
				return CreatedDateTimeOffset.ToDayMonthYear();
			}
		}

		public long CreatedUnixTime
		{
			get
			{
				return CreatedDateTimeOffset.ToUnixTimeMilliseconds();
			}
		}
		
		private DateTimeOffset UpdatedDateTimeOffset { get; }
		
		public string Updated 
		{ 
			get
			{
				return UpdatedDateTimeOffset.ToDayMonthYear();
			} 
		}

		public long UpdatedUnixTime
		{
			get
			{
				return UpdatedDateTimeOffset.ToUnixTimeMilliseconds();
			}
		}
		
		private DateTimeOffset ClosedDateTimeOffset { get; }

		public string Closed
		{
			get
			{
				return ClosedDateTimeOffset.ToDayMonthYear();
			}
		}
		
		private DateTimeOffset ReviewDateTimeOffset { get; }

		public string Review
		{
			get
			{
				return ReviewDateTimeOffset.ToDayMonthYear();
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
		
		/*public string AcademyNames { get; }
		
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
		
		public Tuple<int, IList<string>> RagRating { get; }
		
		public IList<string> RagRatingCss { get; }*/
		
		
		public RatingModel RatingModel { get; }
		
		public IList<RecordModel> RecordsModel { get; }
		
		public HomeModel(string caseUrn, DateTimeOffset created, DateTimeOffset updated, DateTimeOffset closed, DateTimeOffset review,
			string createdBy, string trustName, RatingModel ratingModel, IList<RecordModel> recordsModel) => 
			(CaseUrn, CreatedDateTimeOffset, UpdatedDateTimeOffset, ClosedDateTimeOffset, ReviewDateTimeOffset, CreatedBy, TrustName, RatingModel, RecordsModel) = 
			(caseUrn, created, updated, closed, review, createdBy, trustName, ratingModel, recordsModel);
		
		// public HomeModel(string caseUrn, DateTimeOffset created, DateTimeOffset updated, DateTimeOffset closed, DateTimeOffset review,
		// 	string createdBy, string trustName, string academyNames, string caseType, string caseSubType, 
		// 	Tuple<int, IList<string>> ragRating, IList<string> ragRatingCss) => 
		// 	(CaseUrn, CreatedDateTimeOffset, UpdatedDateTimeOffset, ClosedDateTimeOffset, ReviewDateTimeOffset, CreatedBy, TrustName, AcademyNames, CaseType, CaseSubType, RagRating, RagRatingCss) = 
		// 	(caseUrn, created, updated, closed, review, createdBy, trustName, academyNames, caseType, caseSubType, ragRating, ragRatingCss);
	}
}