﻿using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.NtiUnderConsideration;
using ConcernsCaseWork.Service.Permissions;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly ICasePermissionsService _casePermissionsService;
		private readonly ILogger<IndexPageModel> _logger;

		public NtiUnderConsiderationModel NTIUnderConsiderationModel { get; set; }
		public bool UserCanDelete { get; set; }


		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case");

		public IndexPageModel(INtiUnderConsiderationModelService ntiModelService, ICasePermissionsService casePermissionsService, ILogger<IndexPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_casePermissionsService = casePermissionsService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			try
			{			
				long ntiUnderConsiderationId;
				long caseId;
				_logger.LogInformation("Case::Action::NTI-UC::IndexPageModel::OnGetAsync");

				(caseId, ntiUnderConsiderationId) = GetRouteData();

				NTIUnderConsiderationModel = await _ntiModelService.GetNtiUnderConsiderationViewModel(caseId, ntiUnderConsiderationId);

				if (NTIUnderConsiderationModel == null)
				{
					throw new Exception($"Could not load NTI: UnderConsideration with ID {ntiUnderConsiderationId}");
				}

				UserCanDelete = await _casePermissionsService.UserHasDeletePermissions(caseId);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::NTI-UC::IndexPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}
		
		private (long caseUrn, long ntiUnderConsiderationId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var ntiUnderConsiderationValue = RouteData.Values["ntiUnderConsiderationId"];
			if (ntiUnderConsiderationValue == null || !long.TryParse(ntiUnderConsiderationValue.ToString(), out long ntiUnderConsiderationId) || ntiUnderConsiderationId == 0)
				throw new Exception("NTIUnderConsiderationValue is null or invalid to parse");

			return (caseUrn, ntiUnderConsiderationId);
		}

	}
}