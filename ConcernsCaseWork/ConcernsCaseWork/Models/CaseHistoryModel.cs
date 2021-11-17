using System;
using static ConcernsCaseWork.Extensions.DateExtension;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class CaseHistoryModel
	{
		public DateTimeOffset CreatedAt { get; set; }
		public string Created { get { return CreatedAt.ToUserFriendlyDate(); } }

		public long CaseUrn { get; set; }
		
		public string Action { get; set; }

		public string Title { get; set; }
		
		public string Description { get; set; }
		
		public long Urn { get; set; }
	}
}