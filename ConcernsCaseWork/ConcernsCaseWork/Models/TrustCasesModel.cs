using ConcernsCasework.Service.Status;
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
		private readonly DateTimeOffset closedAtDateTimeOffset;
		private readonly DateTimeOffset createdAtDateTimeOffset;
		
		public long CaseUrn { get; }

		public IList<RecordModel> RecordsModel { get; }

		public RatingModel RatingModel { get; set; }

		public StatusEnum Status { get; private set; }

		public string Created => createdAtDateTimeOffset.ToDayMonthYear();

		public string ClosedAt => closedAtDateTimeOffset.ToDayMonthYear();
			
		public TrustCasesModel(long caseUrn, IList<RecordModel> recordsModel, RatingModel ratingModel, DateTimeOffset createdAt, DateTimeOffset closedAt, StatusEnum status) => 
			(CaseUrn, RecordsModel, RatingModel, createdAtDateTimeOffset, closedAtDateTimeOffset, Status) = 
			(caseUrn, recordsModel, ratingModel, createdAt, closedAt, status);
	}
}