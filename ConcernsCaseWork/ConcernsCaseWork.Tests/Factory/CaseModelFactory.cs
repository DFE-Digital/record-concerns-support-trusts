using ConcernsCaseWork.Models;
using System.Collections.Generic;

namespace ConcernsCaseWork.Tests.Factory
{
	internal static class CaseModelFactory
	{
		public static List<CaseModel> CreateCaseModels()
		{
			return new List<CaseModel>
			{
				new CaseModel("CI-1004634", "-", "Wintermute Academy Trust", 0, 1),
				new CaseModel("CI-1004635", "Safeguarding", "Straylight Academies", 1, 3),
				new CaseModel("CI-1004636", "Finance", "The Linda Lee Academies Trust", 2, 12),
				new CaseModel("CI-1004637", "Governance", "Wintermute Academy Trust", 3, 17),
				new CaseModel("CI-1004638", "Finance", "Armitage Education Trust", 4, 32)
			};
		}
	}
}