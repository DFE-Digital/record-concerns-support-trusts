using ConcernsCaseWork.API.Contracts.TrustFinancialForecast;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.Features.TrustFinancialForecast;

public class GetTrustFinancialForecastById(IConcernsCaseGateway concernsCaseGateway, ITrustFinancialForecastGateway trustFinancialForecastGateway) : IUseCaseAsync<GetTrustFinancialForecastByIdRequest, TrustFinancialForecastResponse>
{
	private readonly IConcernsCaseGateway _concernsCaseGateway = concernsCaseGateway ?? throw new ArgumentNullException(nameof(concernsCaseGateway));
	private readonly ITrustFinancialForecastGateway _trustFinancialForecastGateway = trustFinancialForecastGateway ?? throw new ArgumentNullException(nameof(trustFinancialForecastGateway));

	public async Task<TrustFinancialForecastResponse> Execute(GetTrustFinancialForecastByIdRequest request, CancellationToken cancellationToken)
	{
		EnsureRequestIsValid(request);

		await EnsureCaseExists(request.CaseUrn, cancellationToken);

		var response = await _trustFinancialForecastGateway.GetById(request.TrustFinancialForecastId, cancellationToken);
		if (response == null)
			return null;
		return response.ToResponseModel();
	}

	private static void EnsureRequestIsValid(GetTrustFinancialForecastByIdRequest request)
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