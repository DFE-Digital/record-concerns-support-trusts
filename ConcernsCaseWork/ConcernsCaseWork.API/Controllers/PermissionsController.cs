using Ardalis.GuardClauses;
using Azure;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Enums;
using ConcernsCaseWork.Logging;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ConcernsCaseWork.API.Controllers
{
	[ApiVersion("2.0")]
	[Route("v{version:apiVersion}/permissions/")]
	[ApiController]
	public class PermissionsController : Controller
	{
		private readonly ILogger<ConcernsCaseController> _logger;
		private readonly IGetConcernsCaseByUrn _getConcernsCaseByUrn;

		public PermissionsController(
			ILogger<ConcernsCaseController> logger,
			IGetConcernsCaseByUrn getConcernsCaseByUrn)
		{
			_logger = Guard.Against.Null(logger);
			_getConcernsCaseByUrn = Guard.Against.Null(getConcernsCaseByUrn);
		}

		[HttpPost]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiSingleResponseV2<PermissionQueryResponse>> GetPermissions(PermissionQueryRequest request)
		{
			Guard.Against.Null(request);
			_logger.LogMethodEntered();

			try
			{
				// TODO. Create strategies and validate correctly in another layer
				var casePermissions = request.CaseIds.Select(x => new CasePermissionResponse
				{
					CaseId = x, Permissions = new CasePermission[] { CasePermission.View, CasePermission.Edit }
				}).ToArray();

				var permissions = new PermissionQueryResponse()
				{
					CasePermissionResponses = casePermissions
				};

				var response = new ApiSingleResponseV2<PermissionQueryResponse>(permissions);
				return Ok(response);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred whilst calculating permissions for request");
			}

			return BadRequest();
		}
	}
}
