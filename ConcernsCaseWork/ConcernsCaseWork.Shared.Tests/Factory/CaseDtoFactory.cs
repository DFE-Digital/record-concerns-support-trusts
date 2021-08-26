using Service.TRAMS.Models;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class CaseDtoFactory
	{
		public static List<CaseDto> CreateListCaseDto()
		{
			return new List<CaseDto>
			{
				new CaseDto("CI-1004634", "-", "Wintermute Academy Trust", 0, "09-08-2021", "09-08-2021", "09-08-2021"),
				new CaseDto("CI-1004635", "Safeguarding", "Straylight Academies", 1, "09-08-2021", "09-08-2021", "09-08-2021"),
				new CaseDto("CI-1004636", "Finance", "The Linda Lee Academies Trust", 2, "09-08-2021", "09-08-2021", "09-08-2021"),
				new CaseDto("CI-1004637", "Governance", "Wintermute Academy Trust", 3, "09-08-2021", "09-08-2021", "09-08-2021"),
				new CaseDto("CI-1004638", "Finance", "Armitage Education Trust", 4, "09-08-2021", "09-08-2021", "09-08-2021")
			};
		}
	}
}