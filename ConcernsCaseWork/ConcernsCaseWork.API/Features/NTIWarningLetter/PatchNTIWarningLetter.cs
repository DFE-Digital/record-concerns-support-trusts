using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIWarningLetter
{
	public class PatchNTIWarningLetter : IUseCase<PatchNTIWarningLetterRequest, NTIWarningLetterResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly INTIWarningLetterGateway _gateway;

		public PatchNTIWarningLetter(INTIWarningLetterGateway gateway, IConcernsCaseGateway concernsCaseGateway)
		{
			_gateway = gateway;
			_concernsCaseGateway = concernsCaseGateway;
		}

		public NTIWarningLetterResponse Execute(PatchNTIWarningLetterRequest request)
		{
			return ExecuteAsync(request).Result;
		}

		public async Task<NTIWarningLetterResponse> ExecuteAsync(PatchNTIWarningLetterRequest request)
		{
			var cc = GetCase(request.CaseUrn);

			var patchedNTIWarningLetter = await _gateway.PatchNTIWarningLetter(NTIWarningLetterFactory.CreateDBModel(request));

			cc.CaseLastUpdatedAt = patchedNTIWarningLetter.UpdatedAt;
			await _concernsCaseGateway.UpdateExistingAsync(cc);

			return NTIWarningLetterFactory.CreateResponse(patchedNTIWarningLetter);
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
