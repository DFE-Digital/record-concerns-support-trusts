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

		public IList<RecordModel> RecordsModel { get; }

		public RatingModel RatingModel { get; set; }

		public string Created 
		{ 
			get
			{
				return CreatedAtDateTimeOffset.ToDayMonthYear();
			} 
		}
	
		private DateTimeOffset CreatedAtDateTimeOffset { get; }
		
		public TrustCasesModel(long caseUrn, IList<RecordModel> recordsModel, RatingModel ratingModel, DateTimeOffset createdAt) => 
			(CaseUrn, RecordsModel, RatingModel, CreatedAtDateTimeOffset) = 
			(caseUrn, recordsModel, ratingModel, createdAt);
	}
}