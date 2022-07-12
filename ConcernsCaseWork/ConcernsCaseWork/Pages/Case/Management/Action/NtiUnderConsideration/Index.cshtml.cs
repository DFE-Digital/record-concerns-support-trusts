using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Services.Cases;
using System.Linq;
using Service.Redis.NtiUnderConsideration;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiUnderConsideration
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly INtiUnderConsiderationModelService _ntiModelService;
		private readonly INtiReasonsCachedService _ntiReasonsCachedService;
		private readonly ILogger<IndexPageModel> _logger;

		public NtiModel NTIUnderConsiderationModel { get; set; }

		public IndexPageModel(INtiUnderConsiderationModelService ntiModelService, INtiReasonsCachedService ntiReasonsCachedService, ILogger<IndexPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_ntiReasonsCachedService = ntiReasonsCachedService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			long caseUrn = 0;
			long ntiUnderConsiderationId = 0;

			try
			{
				_logger.LogInformation("Case::Action::NTI-UC::IndexPageModel::OnGetAsync");

				(caseUrn, ntiUnderConsiderationId) = GetRouteData();

				NTIUnderConsiderationModel = await _ntiModelService.GetNtiUnderConsideration(ntiUnderConsiderationId);

				if (NTIUnderConsiderationModel == null)
				{
					throw new Exception($"Could not load NTI: UnderConsideration with ID {ntiUnderConsiderationId}");
				}
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