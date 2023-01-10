using ConcernsCaseWork.API.Authorization;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.Exceptions;
using ConcernsCaseWork.Data.Gateways;
using System.Security.Claims;

namespace ConcernsCaseWork.API.UseCases.Permissions
{
	public class GetCasePermissions : IUseCase<GetCasePermissionsParams, GetCasePermissionsResponse>
	{
		private readonly IConcernsCaseGateway _concernsCaseGateway;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

		public GetCasePermissions(
			IConcernsCaseGateway concernsCaseGateway,
			IClaimsPrincipalHelper claimsPrincipalHelper)
		{
			_concernsCaseGateway = concernsCaseGateway;
			_claimsPrincipalHelper = claimsPrincipalHelper;
		}

		public GetCasePermissionsResponse Execute(GetCasePermissionsParams parameters)
		{
			var concernsCase = _concernsCaseGateway.GetConcernsCaseByUrn(parameters.Id);

			if (concernsCase == null) throw new NotFoundException($"Concern with id {parameters.Id}");

			var builder = new GetCasePermissionsResponseBuilder();
			var builderParameters = new GetCasePermissionsResponseBuilderParams()
			{
				CaseOwner = concernsCase.CreatedBy,
				Username = _claimsPrincipalHelper.GetPrincipalName(parameters.User),
				IsAdmin = _claimsPrincipalHelper.IsAdmin(parameters.User)
			};

			var result = builder.Build(builderParameters);

			return result;
		}
	}

	public class GetCasePermissionsParams
	{
		public int Id { get; set; }
		public ClaimsPrincipal User { get; set; }
	}
}
