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
		
		public RatingModel RatingModel { get; }
		
		public IList<RecordModel> RecordsModel { get; }
		
		public HomeModel(string caseUrn, DateTimeOffset created, DateTimeOffset updated, DateTimeOffset closed, DateTimeOffset review,
			string createdBy, string trustName, RatingModel ratingModel, IList<RecordModel> recordsModel) => 
			(CaseUrn, CreatedDateTimeOffset, UpdatedDateTimeOffset, ClosedDateTimeOffset, ReviewDateTimeOffset, CreatedBy, TrustName, RatingModel, RecordsModel) = 
			(caseUrn, created, updated, closed, review, createdBy, trustName, ratingModel, recordsModel);
	}
}