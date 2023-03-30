using Ardalis.GuardClauses;
using Azure;
using ConcernsCaseWork.API.Contracts.Permissions;
using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.API.UseCases.Permissions.Cases;
using ConcernsCaseWork.API.UseCases.Permissions.Cases.Strategies;
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
		private readonly IGetCasePermissionsUseCase _getCasePermissionsUseCase;
		private readonly ILogger<PermissionsController> _logger;
		private readonly IServerUserInfoService _userInfoService;

		public PermissionsController(
			ILogger<PermissionsController> logger,
			IServerUserInfoService userInfoService,
			IGetCasePermissionsUseCase getCasePermissionsUseCase)
		{
			_logger = Guard.Against.Null(logger);
			_userInfoService = Guard.Against.Null(userInfoService);
			_getCasePermissionsUseCase = Guard.Against.Null(getCasePermissionsUseCase);
		}

		[HttpPost]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<PermissionQueryResponse>>> GetPermissions(PermissionQueryRequest request, CancellationToken cancellationToken = default)
		{
			Guard.Against.Null(request);

			_logger.LogMethodEntered();

			if (_userInfoService.UserInfo == null)
			{
				_logger.LogError("User information is null, cannot determined if current user owns cases or has permissions");
				return BadRequest("User information is null, cannot determined if current user owns cases or has permissions");
			}

			try
			{
				var userInfo = _userInfoService.UserInfo;
				PermissionQueryResponse allCasePermissions = await _getCasePermissionsUseCase.Execute((request.CaseIds, userInfo), cancellationToken);

				var response = new ApiSingleResponseV2<PermissionQueryResponse>(allCasePermissions);
				return Ok(response);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An exception occurred whilst calculating permissions for request");
				throw;
			}
		}
	}
}
