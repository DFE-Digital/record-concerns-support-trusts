using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Service.TRAMS.Decision;
﻿using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Decision;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Services.Decisions
{
	public class DecisionMapping
	{
		public static ActionSummaryModel ToActionSummary(GetDecisionResponseDto decision)
		{
			var result = new ActionSummaryModel()
			{
				OpenedDate = decision.CreatedAt.ToDayMonthYear(),
				ClosedDate = decision.ClosedAt?.ToDayMonthYear(),
				Name = $"Decision: {decision.Title}",
				StatusName = EnumHelper.GetEnumDescription(decision.Status),
				RelativeUrl = $"/case/{decision.ConcernsCaseUrn}/management/action/decision/{decision.DecisionId}"
			};

			return result;
		}
	}
}
