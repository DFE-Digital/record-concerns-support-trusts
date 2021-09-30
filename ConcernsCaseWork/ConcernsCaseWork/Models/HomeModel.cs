using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class HomeModel
	{
		public string CaseUrn { get; }
		
		public string Created { get; }
		
		public string Updated { get; }
		
		public string Closed { get; }
		
		public string Review { get; }
		
		public string TrustName { get; }
		
		public string AcademyNames { get; }
		
		public string CaseTypeDescription
		{
			get
			{
				return string.IsNullOrEmpty(CaseSubType) ? CaseType : CaseSubType;
			}
		}
		
		public string CaseType { get; }
		
		public string CaseSubType { get; }
		
		public IList<string> RagRating { get; }
		
		public IList<string> RagRatingCss { get; }
		
		public HomeModel(string caseUrn, string created, string updated, string closed, string review,
			string trustName, string academyNames, string caseType, string caseSubType, 
			IList<string> ragRating, IList<string> ragRatingCss) => 
			(CaseUrn, Created, Updated, Closed, Review, TrustName, AcademyNames, CaseType, CaseSubType, RagRating, RagRatingCss) = 
			(caseUrn, created, updated, closed, review, trustName, academyNames, caseType, caseSubType, ragRating, ragRatingCss);
	}
}