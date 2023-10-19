using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.Features.TrustFinancialForecast;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.TrustFinancialForecast;

public class GetTrustFinancialForecastsForCase : IUseCaseAsync<GetTrustFinancialForecastsForCaseRequest, IEnumerable<TrustFinancialForecastResponse>>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;
	private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway;

	public GetTrustFinancialForecastsForCase(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway)
	{
		_concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
		_trustFinancialForecastGateway = trustFinancialForecastGateway ?? throw new ArgumentNullException(nameof(trustFinancialForecastGateway));
	}

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