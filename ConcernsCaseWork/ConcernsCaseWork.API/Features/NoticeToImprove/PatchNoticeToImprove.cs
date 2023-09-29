using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.RequestModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.NoticeToImprove;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
{
	public class PatchNoticeToImprove : IUseCase<PatchNoticeToImproveRequest, NoticeToImproveResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly INoticeToImproveGateway _gateway;

		public PatchNoticeToImprove(INoticeToImproveGateway gateway, IConcernsCaseGateway concernsCaseGateway)
		{
			_gateway = gateway;
			_concernsCaseGateway = concernsCaseGateway;
		}

		public NoticeToImproveResponse Execute(PatchNoticeToImproveRequest request)
		{
			return ExecuteAsync(request).Result;
		}

		public async Task<NoticeToImproveResponse> ExecuteAsync(PatchNoticeToImproveRequest request)
		{
			var cc = GetCase(request.CaseUrn);

			var patchedNoticeToImprove = await _gateway.PatchNoticeToImprove(NoticeToImproveFactory.CreateDBModel(request));

			cc.CaseLastUpdatedAt = patchedNoticeToImprove.UpdatedAt;

			await _concernsCaseGateway.UpdateExistingAsync(cc);

			return NoticeToImproveFactory.CreateResponse(patchedNoticeToImprove);
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
