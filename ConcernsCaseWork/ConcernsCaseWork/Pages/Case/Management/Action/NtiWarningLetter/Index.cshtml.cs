using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using System.Linq;
using Service.Redis.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
using System.Collections.Generic;
using ConcernsCasework.Service.NtiWarningLetter;
using ConcernsCaseWork.Mappers;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly INtiWarningLetterStatusesCachedService _ntiWarningLetterStatusesCachedService;
		private readonly INtiWarningLetterReasonsCachedService _ntiWarningLetterReasonsCachedService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiWarningLetterConditionsCachedService _ntiWarningLetterConditionsCachedService;
		private readonly ILogger<IndexPageModel> _logger;

		public NtiWarningLetterModel NtiWarningLetterModel { get; set; }
		public ICollection<NtiWarningLetterStatusDto> NtiWarningLetterStatuses { get; set; }
		public ICollection<NtiWarningLetterReasonDto> NtiWarningLetterReasons { get; private set; }
		public ICollection<NtiWarningLetterConditionDto> NtiWarningLetterConditions { get; private set; }

		public IndexPageModel(INtiWarningLetterStatusesCachedService ntiWarningLetterStatusesCachedService,
			INtiWarningLetterReasonsCachedService ntiWarningLetterReasonsCachedService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiWarningLetterConditionsCachedService ntiWarningLetterConditionsCachedService,
			ILogger<IndexPageModel> logger)
		{
			_ntiWarningLetterStatusesCachedService = ntiWarningLetterStatusesCachedService;
			_ntiWarningLetterReasonsCachedService = ntiWarningLetterReasonsCachedService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiWarningLetterConditionsCachedService = ntiWarningLetterConditionsCachedService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			long caseUrn = 0;
			long ntiWarningLetterId = 0;

			try
			{
				_logger.LogInformation("Case::Action::NTI-Warning letter::IndexPageModel::OnGetAsync");

				(caseUrn, ntiWarningLetterId) = GetRouteData();

				NtiWarningLetterModel = await GetWarningLetterModel(ntiWarningLetterId);

				if (NtiWarningLetterModel == null)
				{
					throw new Exception($"Could not load NTI: Warning letter with ID {ntiWarningLetterId}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::NTI-Warning letter::IndexPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		private async Task<NtiWarningLetterModel> GetWarningLetterModel(long ntiWarningLetterId)
		{
			var wl = await _ntiWarningLetterModelService.GetNtiWarningLetterId(ntiWarningLetterId);
			NtiWarningLetterStatuses = await _ntiWarningLetterStatusesCachedService.GetAllStatusesAsync();

			if (wl != null)
			{
				if (wl.Reasons?.Any() == true)
				{
					NtiWarningLetterReasons = await _ntiWarningLetterReasonsCachedService.GetAllReasonsAsync();
					wl.Reasons = NtiWarningLetterReasons.Where(r => wl.Reasons.Any(wlr => wlr.Id == r.Id))?.Select(r => NtiWarningLetterMappers.ToServiceModel(r)).ToArray();
				}

				if (wl.Conditions?.Any() == true)
				{
					NtiWarningLetterConditions = await _ntiWarningLetterConditionsCachedService.GetAllConditionsAsync();
					wl.Conditions = NtiWarningLetterConditions.Where(c => wl.Conditions.Any(wlc => c.Id == wlc.Id)).Select(c => NtiWarningLetterMappers.ToServiceModel(c)).ToArray();
				}
			}

			return wl;
		}

		private (long caseUrn, long ntiUnderConsiderationId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var ntiUnderConsiderationValue = RouteData.Values["ntiWarningLetterId"];
			if (ntiUnderConsiderationValue == null || !long.TryParse(ntiUnderConsiderationValue.ToString(), out long ntiUnderConsiderationId) || ntiUnderConsiderationId == 0)
				throw new Exception("Nti Warning letter Id is null or invalid to parse");

			return (caseUrn, ntiUnderConsiderationId);
		}


	}
}