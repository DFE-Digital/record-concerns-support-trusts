using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.UseCases.Permissions;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Mvc;
using ConcernsCaseWork.API.Contracts.Decisions.Outcomes;
using ConcernsCaseWork.API.ResponseModels;

namespace ConcernsCaseWork.API.Controllers
{
	[ApiVersion("2.0")]
	[ApiController]
	[Route("v{version:apiVersion}/permissions/cases")]
	public class CasePermissionsController : ControllerBase
	{
		private readonly IUseCase<GetCasePermissionsParams, GetCasePermissionsResponse> _getCasePermissionsUseCase;
		private readonly ILogger<ConcernsStatusController> _logger;

		public CasePermissionsController(
			IUseCase<GetCasePermissionsParams, GetCasePermissionsResponse> getCasePermissionsUseCase,
			ILogger<ConcernsStatusController> logger)
		{
			_getCasePermissionsUseCase = getCasePermissionsUseCase;
			_logger = logger;
		}

		[HttpGet]
		[Route("{id}")]
		[MapToApiVersion("2.0")]
		public IActionResult Get(int id)
		{
			_logger.LogInformation($"Getting permissions for case {id}");

			var parameters = new GetCasePermissionsParams() { Id = id, User = User };

			var permissions = _getCasePermissionsUseCase.Execute(parameters);

			_logger.LogInformation($"Retrieved permissions for case {id}");

			var result = new ApiSingleResponseV2<GetCasePermissionsResponse>(permissions);

			return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
		}
	}
}
