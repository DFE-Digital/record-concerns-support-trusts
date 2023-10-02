using ConcernsCaseWork.API.Contracts.NtiWarningLetter;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIWarningLetter
{
	public class CreateNTIWarningLetter : IUseCase<CreateNTIWarningLetterRequest, NTIWarningLetterResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly INTIWarningLetterGateway _gateway;

		public CreateNTIWarningLetter(INTIWarningLetterGateway gateway, IConcernsCaseGateway concernsCaseGateway)
		{
			_gateway = gateway;
			_concernsCaseGateway = concernsCaseGateway;
		}

		public NTIWarningLetterResponse Execute(CreateNTIWarningLetterRequest request)
		{
			return ExecuteAsync(request).Result;
		}

		public async Task<NTIWarningLetterResponse> ExecuteAsync(CreateNTIWarningLetterRequest request)
		{
			var cc = GetCase(request.CaseUrn);

			var dbModel = NTIWarningLetterFactory.CreateDBModel(request);

			var createdNTIWarningLetter = await _gateway.CreateNTIWarningLetter(dbModel);

			cc.CaseLastUpdatedAt = dbModel.CreatedAt;
			await _concernsCaseGateway.UpdateExistingAsync(cc);

			return NTIWarningLetterFactory.CreateResponse(createdNTIWarningLetter);
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
