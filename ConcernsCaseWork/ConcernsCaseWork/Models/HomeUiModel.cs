namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class HomeUiModel
	{
		public string CaseId { get; }
		
		public string Created { get; }
		
		public string Updated { get; }
		
		public string TrustName { get; }
		
		public string AcademyNames { get; }
		
		public string CaseType { get; }
		
		public string RagRating { get; }
		
		public string RagRatingCss { get; }
		
		public HomeUiModel(string caseId, string created, string updated, string trustName, string academyNames, 
			string caseType, string ragRating, string ragRatingCss) => 
			(CaseId, Created, Updated, TrustName, AcademyNames, CaseType, RagRating, RagRatingCss) = 
			(caseId, created, updated, trustName, academyNames, caseType, ragRating, ragRatingCss);

	}
}