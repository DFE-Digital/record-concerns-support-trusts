using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
{
	public class CreateNTIUnderConsideration : IUseCase<CreateNTIUnderConsiderationRequest, NTIUnderConsiderationResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly INTIUnderConsiderationGateway _gateway;

		public CreateNTIUnderConsideration(INTIUnderConsiderationGateway gateway, IConcernsCaseGateway concernsCaseGateway)
		{
			_gateway = gateway;
			_concernsCaseGateway = concernsCaseGateway;

		}

		public NTIUnderConsiderationResponse Execute(CreateNTIUnderConsiderationRequest request)
		{
			return ExecuteAsync(request).Result;
		}

		public async Task<NTIUnderConsiderationResponse> ExecuteAsync(CreateNTIUnderConsiderationRequest request)
		{
			var cc = GetCase(request.CaseUrn);

			var dbModel = NTIUnderConsiderationFactory.CreateDBModel(request);

			var createdNTIUnderConsideration = await _gateway.CreateNTIUnderConsideration(dbModel);

			cc.CaseLastUpdatedAt = dbModel.CreatedAt;
			await _concernsCaseGateway.UpdateExistingAsync(cc);

			return NTIUnderConsiderationFactory.CreateResponse(createdNTIUnderConsideration);
		}

		private ConcernsCase GetCase(int caseUrn)
		{
			var cc = _concernsCaseGateway.GetConcernsCaseByUrn(caseUrn);
			if (cc == null)
				throw new NotFoundException($"Concerns Case {caseUrn} not found");
			return cc;
		}
	}
}
