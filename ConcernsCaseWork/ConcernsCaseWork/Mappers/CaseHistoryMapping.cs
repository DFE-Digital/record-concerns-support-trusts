using ConcernsCasework.Service.Cases;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Mappers
{
	public static class CaseHistoryMapping
	{
		private static readonly IDictionary<CaseHistoryEnum, CaseHistoryMapper> _caseHistoryMap = new Dictionary<CaseHistoryEnum, CaseHistoryMapper>()
		{
			{ CaseHistoryEnum.Case, new CaseHistoryMapper { Title = "Case", Description = "Case created" } },
			{ CaseHistoryEnum.Concern, new CaseHistoryMapper { Title = "Concern", Description = "Concern created" } },
			{ CaseHistoryEnum.RiskRating, new CaseHistoryMapper { Title = "Risk Rating", Description = "Changes to Risk Rating" } },
			{ CaseHistoryEnum.DirectionOfTravel, new CaseHistoryMapper { Title = "Direction of Travel", Description = "Changes to Direction of Travel" } },
			{ CaseHistoryEnum.Issue, new CaseHistoryMapper { Title = "Issue", Description = "Changes to Issue narrative" } },
			{ CaseHistoryEnum.CurrentStatus, new CaseHistoryMapper { Title = "Current Status", Description = "Changes to Current Status narrative" } },
			{ CaseHistoryEnum.CaseAim, new CaseHistoryMapper { Title = "Case Aim", Description = "Changes to Case Aim narrative" } },
			{ CaseHistoryEnum.DeEscalationPoint, new CaseHistoryMapper { Title = "De-escalation Point", Description = "Changes to De-escalation narrative" } },
			{ CaseHistoryEnum.NextSteps, new CaseHistoryMapper { Title = "Next Steps", Description = "Changes to Next Steps narrative" } },
			{ CaseHistoryEnum.ClosedForMonitoring, new CaseHistoryMapper { Title = "Closed for Monitoring", Description = "Set to Closed for Monitoring" } },
			{ CaseHistoryEnum.Closed, new CaseHistoryMapper { Title = "Closed", Description = "Case closed" } }
		};

		public static CreateCaseHistoryDto BuildCaseHistoryDto(CaseHistoryEnum caseHistoryEnum, long caseUrn)
		{
			if (_caseHistoryMap.TryGetValue(caseHistoryEnum, out CaseHistoryMapper caseHistoryMapper))
			{
				return new CreateCaseHistoryDto(DateTimeOffset.Now, caseUrn, caseHistoryEnum.ToString(), caseHistoryMapper.Title, caseHistoryMapper.Description);
			}

			throw new Exception("CaseHistory not found");
		}
			
		private class CaseHistoryMapper
		{
			public string Title { get; set; }
			public string Description { get; set; }
		}
	}
}