using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;

namespace ConcernsCaseWork.API.Factories.Concerns.Decisions;

public interface ICloseDecisionResponseFactory
{
	public CloseDecisionResponse Create(int concernsCaseUrn, int decisionId);
}