using Ardalis.GuardClauses;
using ConcernsCaseWork.Data.Gateways;

namespace ConcernsCaseWork.API.UseCases;

public class GetOwnersOfOpenCases : IGetOwnersOfOpenCases
{
	private readonly IConcernsCaseGateway _concernsCaseGateway;

	public GetOwnersOfOpenCases(IConcernsCaseGateway concernsCaseGateway)
	{
		_concernsCaseGateway = Guard.Against.Null(concernsCaseGateway);
	}
	public Task<string[]> Execute(CancellationToken cancellationToken)
	{
		return _concernsCaseGateway.GetOwnersOfOpenCases(cancellationToken);
	}
}

public interface IGetOwnersOfOpenCases
{
	Task<string[]> Execute(CancellationToken cancellationToken);
} 