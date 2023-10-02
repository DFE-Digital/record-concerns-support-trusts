using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.UnderConsideration;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
{
	public class PatchNTIUnderConsideration : IUseCase<PatchNTIUnderConsiderationRequest, NTIUnderConsiderationResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly INTIUnderConsiderationGateway _gateway;

		public PatchNTIUnderConsideration(INTIUnderConsiderationGateway gateway, IConcernsCaseGateway concernsCaseGateway)
		{
			_gateway = gateway;
			_concernsCaseGateway = concernsCaseGateway;
		}

		public NTIUnderConsiderationResponse Execute(PatchNTIUnderConsiderationRequest request)
		{
			return ExecuteAsync(request).Result;
		}

		public async Task<NTIUnderConsiderationResponse> ExecuteAsync(PatchNTIUnderConsiderationRequest request)
		{
			var cc = GetCase(request.CaseUrn);
			var patchedNTI = await _gateway.PatchNTIUnderConsideration(NTIUnderConsiderationFactory.CreateDBModel(request));

			cc.CaseLastUpdatedAt = patchedNTI.UpdatedAt;
			await _concernsCaseGateway.UpdateExistingAsync(cc);

			return NTIUnderConsiderationFactory.CreateResponse(patchedNTI);
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
