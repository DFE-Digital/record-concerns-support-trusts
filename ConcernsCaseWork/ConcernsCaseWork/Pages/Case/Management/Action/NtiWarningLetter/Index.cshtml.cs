using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using System.Linq;
using ConcernsCaseWork.Services.NtiWarningLetter;
using System.Collections.Generic;
using ConcernsCaseWork.Service.NtiWarningLetter;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.NtiWarningLetter;
using Microsoft.CodeAnalysis.Operations;
using ConcernsCaseWork.Logging;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly INtiWarningLetterReasonsCachedService _ntiWarningLetterReasonsCachedService;
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiWarningLetterConditionsCachedService _ntiWarningLetterConditionsCachedService;
		private readonly ILogger<IndexPageModel> _logger;

		public NtiWarningLetterModel NtiWarningLetterModel { get; set; }
		public ICollection<NtiWarningLetterReasonDto> NtiWarningLetterReasons { get; private set; }
		public ICollection<NtiWarningLetterConditionDto> NtiWarningLetterConditions { get; private set; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case");

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseId { get; set; }

		[BindProperty(SupportsGet = true, Name = "ntiWarningLetterId")]
		public int NtiWarningLetterId { get; set; }

		public IndexPageModel(
			INtiWarningLetterReasonsCachedService ntiWarningLetterReasonsCachedService,
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiWarningLetterConditionsCachedService ntiWarningLetterConditionsCachedService,
			ILogger<IndexPageModel> logger)
		{
			_ntiWarningLetterReasonsCachedService = ntiWarningLetterReasonsCachedService;
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiWarningLetterConditionsCachedService = ntiWarningLetterConditionsCachedService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				NtiWarningLetterModel = await GetWarningLetterModel(CaseId, NtiWarningLetterId);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}
		}

		private async Task<NtiWarningLetterModel> GetWarningLetterModel(long caseId, long ntiWarningLetterId)
		{
			var wl = await _ntiWarningLetterModelService.GetNtiWarningLetterViewModel(caseId, ntiWarningLetterId);

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
	}
}