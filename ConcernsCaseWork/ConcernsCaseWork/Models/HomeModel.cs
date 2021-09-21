﻿using System.Collections.Generic;

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
		
		public string TrustName { get; }
		
		public string AcademyNames { get; }
		
		public string CaseType { get; }
		
		public string CaseSubType { get; }
		
		public IList<string> RagRating { get; }
		
		public IList<string> RagRatingCss { get; }
		
		public HomeModel(string caseUrn, string created, string updated, string closed, string trustName, string academyNames, 
			string caseType, string caseSubType, IList<string> ragRating, IList<string> ragRatingCss) => 
			(CaseUrn, Created, Updated, Closed, TrustName, AcademyNames, CaseType, CaseSubType, RagRating, RagRatingCss) = 
			(caseUrn, created, updated, closed, trustName, academyNames, caseType, caseSubType, ragRating, ragRatingCss);
	}
}