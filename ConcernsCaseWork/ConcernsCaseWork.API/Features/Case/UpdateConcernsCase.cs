using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case
{
	public interface IUpdateConcernsCase
	{
		ConcernsCaseResponse Execute(int urn, ConcernCaseRequest request);
	}

	public class UpdateConcernsCase(
		IConcernsCaseGateway concernsCaseGateway) : IUpdateConcernsCase
	{
		public ConcernsCaseResponse Execute(int urn, ConcernCaseRequest request)
		{
			var currentConcernsCase = concernsCaseGateway.GetConcernsCaseByUrn(urn);
			var concernsCaseToUpdate = ConcernsCaseFactory.Update(currentConcernsCase, request);
			var updatedConcernsCase = concernsCaseGateway.Update(concernsCaseToUpdate);
			return ConcernsCaseResponseFactory.Create(updatedConcernsCase);
		}
	}
}