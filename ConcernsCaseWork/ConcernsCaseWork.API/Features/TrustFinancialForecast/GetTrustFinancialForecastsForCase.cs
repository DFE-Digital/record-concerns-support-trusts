using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.TrustFinancialForecast;

public class GetTrustFinancialForecastsForCase(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway) : IUseCaseAsync<GetTrustFinancialForecastsForCaseRequest, IEnumerable<TrustFinancialForecastResponse>>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
	private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway = trustFinancialForecastGateway ?? throw new ArgumentNullException(nameof(trustFinancialForecastGateway));

	public async Task<IEnumerable<TrustFinancialForecastResponse>> Execute(GetTrustFinancialForecastsForCaseRequest request, CancellationToken cancellationToken)
	{
		EnsureRequestIsValid(request);

		await EnsureCaseExists(request.CaseUrn, cancellationToken);

		var response = await _trustFinancialForecastGateway.GetAllForCase(request.CaseUrn, cancellationToken);

		return response.Select(r => r.ToResponseModel());
	}

	private static void EnsureRequestIsValid(GetTrustFinancialForecastsForCaseRequest request)
	{
		if (request is null)
			throw new ArgumentNullException(nameof(request));

		if (!request.IsValid())
			throw new ArgumentException("Request is not valid", nameof(request));
	}

	private async Task EnsureCaseExists(int caseUrn, CancellationToken cancellationToken)
	{
		if (!await _concernsCaseGateway.CaseExists(caseUrn, cancellationToken))
			throw new NotFoundException($"Concerns Case {caseUrn} not found");
	}
}