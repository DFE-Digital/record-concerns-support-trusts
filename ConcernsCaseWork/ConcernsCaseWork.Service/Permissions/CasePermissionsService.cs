using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Service.Base;
using Microsoft.Extensions.Logging;

namespace ConcernsCaseWork.Service.Permissions
{
	public class CasePermissionsService : ConcernsAbstractService, ICasePermissionsService
	{
		private readonly ILogger<CasePermissionsService> _logger;

		public CasePermissionsService(
			IHttpClientFactory clientFactory,
			ILogger<CasePermissionsService> logger,
			ICorrelationContext correlationContext) : base(clientFactory, logger, correlationContext)
		{
			_logger = logger;
		}

		public async Task<GetCasePermissionsResponse> GetCasePermissions(long id)
		{
			_logger.LogMethodEntered();
			_logger.LogInformation($"Client getting permissions for case {id}");

			var result = new GetCasePermissionsResponse()
			{
				Permissions = new List<CasePermission>() { CasePermission.Edit }
			};

			_logger.LogInformation($"Client retrieved permissions for case {id}");

			return result;
		}
	}

	public interface ICasePermissionsService
	{
		public Task<GetCasePermissionsResponse> GetCasePermissions(long id);
	}
}
