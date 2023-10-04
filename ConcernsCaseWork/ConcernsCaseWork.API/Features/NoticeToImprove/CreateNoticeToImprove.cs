using ConcernsCaseWork.API.Contracts.NoticeToImprove;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NoticeToImprove
{
	public class CreateNoticeToImprove : IUseCase<CreateNoticeToImproveRequest, NoticeToImproveResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly INoticeToImproveGateway _gateway;

		public CreateNoticeToImprove(INoticeToImproveGateway gateway, IConcernsCaseGateway concernsCaseGateway)
		{
			_gateway = gateway;
			_concernsCaseGateway = concernsCaseGateway;
		}

		public NoticeToImproveResponse Execute(CreateNoticeToImproveRequest request)
		{
			return ExecuteAsync(request).Result;
		}

		public async Task<NoticeToImproveResponse> ExecuteAsync(CreateNoticeToImproveRequest request)
		{
			var cc = GetCase(request.CaseUrn);
			var dbModel = NoticeToImproveFactory.CreateDBModel(request);

			var createdNoticeToImprove = await _gateway.CreateNoticeToImprove(dbModel);

			cc.CaseLastUpdatedAt = dbModel.CreatedAt;

			await _concernsCaseGateway.UpdateExistingAsync(cc);

			return NoticeToImproveFactory.CreateResponse(createdNoticeToImprove);
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
