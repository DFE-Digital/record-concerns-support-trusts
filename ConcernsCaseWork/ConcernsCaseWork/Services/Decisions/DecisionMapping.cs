using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models.CaseActions;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Service.TRAMS.Decision;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Services.Decisions
{
	public class DecisionMapping
	{
		public static DecisionModel MapDtoToModel(GetDecisionResponseDto decision)
		{
			var result = new DecisionModel()
			{
				CreatedAt = decision.CreatedAt.Date,
				ClosedAt = decision.ClosedAt?.Date,
				CaseUrn = decision.ConcernsCaseUrn,
				Id = decision.DecisionId,
				Title = decision.Title
			};

			return result;
		}

		public static List<DecisionModel> MapDtoToModel(List<GetDecisionResponseDto> decisions)
		{
			return decisions.Select(d => MapDtoToModel(d)).ToList();
		}
	}
}
