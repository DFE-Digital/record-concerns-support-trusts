using Ardalis.GuardClauses;
using Azure;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Enums;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.UserContext;
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
		private readonly ICaseActionPermissionStrategyRoot _caseActionPermissionStrategyRoot;
		private readonly ILogger<ConcernsCaseController> _logger;
		private readonly IGetConcernsCaseByUrn _getConcernsCaseByUrn;
		private readonly IServerUserInfoService _userInfoService;

		public PermissionsController(
			ILogger<ConcernsCaseController> logger,
			IGetConcernsCaseByUrn getConcernsCaseByUrn,
			IServerUserInfoService userInfoService,
			ICaseActionPermissionStrategyRoot caseActionPermissionStrategyRoot)
		{
			_logger = Guard.Against.Null(logger);
			_getConcernsCaseByUrn = Guard.Against.Null(getConcernsCaseByUrn);
			_userInfoService = Guard.Against.Null(userInfoService);
			_caseActionPermissionStrategyRoot = Guard.Against.Null(caseActionPermissionStrategyRoot);
		}

		[HttpPost]
		[MapToApiVersion("2.0")]
		public ActionResult<ApiSingleResponseV2<PermissionQueryResponse>> GetPermissions(PermissionQueryRequest request)
		{
			Guard.Against.Null(request);

			_logger.LogMethodEntered();

			if (_userInfoService.UserInfo == null)
			{
				throw new NullReferenceException("User information is null, cannot determined if current user owns cases or has permissions");
			}

			try
			{
				List<CasePermissionResponse> allCasePermissions = new(request.CaseIds.Length);
				foreach (long caseId in request.CaseIds)
				{
					// TODO: shouldn't need to cast this to an int, incorrect types need to be sorted out.
					// TODO: optimize query to get all cases requested in one db query
					ConcernsCaseResponse @case = this._getConcernsCaseByUrn.Execute((int)caseId);

					var permittedCaseActions = _caseActionPermissionStrategyRoot.GetPermittedCaseActions(@case, _userInfoService.UserInfo);
					allCasePermissions.Add(new CasePermissionResponse() { CaseId = caseId, Permissions = permittedCaseActions });
				}

				var response = new ApiSingleResponseV2<PermissionQueryResponse>(new PermissionQueryResponse(allCasePermissions));
				return Ok(response);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred whilst calculating permissions for request");
				throw;
			}

			return BadRequest();
		}
	}
}
