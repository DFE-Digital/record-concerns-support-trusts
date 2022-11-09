using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions
{
	public interface IUpdateDecisionResponseFactory
	{
		public UpdateDecisionResponse Create(int concernsCaseUrn, int decisionId);
	}
}
