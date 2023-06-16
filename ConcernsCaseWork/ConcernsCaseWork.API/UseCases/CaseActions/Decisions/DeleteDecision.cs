using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Factories.Concerns.Decisions;
using ConcernsCaseWork.Data.Gateways;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.API.UseCases.CaseActions.Decisions
{
	public class DeleteDecision : IUseCaseAsync<DeleteDecisionRequest, DeleteDecisionResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly IDecisionFactory _decisionFactory;
		private readonly ICreateDecisionResponseFactory _createDecisionResponseFactory;

		public DeleteDecision(IConcernsCaseGateway concernsCaseGateway, IDecisionFactory decisionFactory, ICreateDecisionResponseFactory createDecisionResponseFactory)
		{
			_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
			_decisionFactory = decisionFactory ?? throw new ArgumentNullException(nameof(decisionFactory));
			_createDecisionResponseFactory = createDecisionResponseFactory ?? throw new ArgumentNullException(nameof(createDecisionResponseFactory));
		}

		public Task<DeleteDecisionResponse> Execute(DeleteDecisionRequest request, CancellationToken cancellationToken)
		{
			_ = request ?? throw new ArgumentNullException(nameof(request));

			async Task<DeleteDecisionResponse> DoWork()
			{
				var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(request.ConcernsCaseUrn) ??
								   throw new InvalidOperationException(
									   $"The concerns case for urn {request.ConcernsCaseUrn}, was not found");


				concernsCase.DeleteDecision(request.DecisionId, DateTimeOffset.Now);


				cancellationToken.ThrowIfCancellationRequested();

				_concernsCaseGateway.SaveConcernsCase(concernsCase);

				return null;
			}

			return DoWork();
		}
	}

	public class DeleteDecisionRequest
	{

		[Range(1, int.MaxValue, ErrorMessage = "The ConcernsCaseUrn must be greater than zero")]
		public int ConcernsCaseUrn { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "The DecisionId must be greater than zero")]
		public int DecisionId { get; set; }

		public DeleteDecisionRequest(int concernsCaseUrn, int decisionId)
		{
			_ = concernsCaseUrn <= 0 ? throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn)) : concernsCaseUrn;
			_ = decisionId <= 0 ? throw new ArgumentOutOfRangeException(nameof(concernsCaseUrn)) : decisionId;

			ConcernsCaseUrn = concernsCaseUrn;
			DecisionId = decisionId;
		}
	}

	public class DeleteDecisionResponse
	{

	}
}
