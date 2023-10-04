using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.Case
{
	public interface ICreateConcernsCase
	{
		public ConcernsCaseResponse Execute(ConcernCaseRequest request);
	}

	public class CreateConcernsCase : ICreateConcernsCase
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;

		public CreateConcernsCase(IConcernsCaseGateway concernsCaseGateway)
		{
			_concernsCaseGateway = concernsCaseGateway;
		}

		public ConcernsCaseResponse Execute(ConcernCaseRequest request)
		{
			var concernsCaseToCreate = ConcernsCaseFactory.Create(request);
			var createdConcernsCase = _concernsCaseGateway.SaveConcernsCase(concernsCaseToCreate);
			return ConcernsCaseResponseFactory.Create(createdConcernsCase);
		}
	}
}