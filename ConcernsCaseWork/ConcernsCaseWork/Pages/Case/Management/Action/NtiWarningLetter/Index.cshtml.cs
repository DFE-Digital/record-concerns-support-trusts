using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.NtiWarningLetter;
using ConcernsCaseWork.Services.NtiWarningLetter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.NtiWarningLetter
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly INtiWarningLetterModelService _ntiWarningLetterModelService;
		private readonly INtiWarningLetterConditionsService _ntiWarningLetterConditionsService;
		private readonly ILogger<IndexPageModel> _logger;

		public NtiWarningLetterModel NtiWarningLetterModel { get; set; }
		public ICollection<NtiWarningLetterConditionDto> NtiWarningLetterConditions { get; private set; }
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case");

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseId { get; set; }

		[BindProperty(SupportsGet = true, Name = "ntiWarningLetterId")]
		public int NtiWarningLetterId { get; set; }

		public IndexPageModel(
			INtiWarningLetterModelService ntiWarningLetterModelService,
			INtiWarningLetterConditionsService ntiWarningLetterConditionsService,
			ILogger<IndexPageModel> logger)
		{
			_ntiWarningLetterModelService = ntiWarningLetterModelService;
			_ntiWarningLetterConditionsService = ntiWarningLetterConditionsService;
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
				if (wl.Conditions?.Any() == true)
				{
					NtiWarningLetterConditions = await _ntiWarningLetterConditionsService.GetAllConditionsAsync();
					wl.Conditions = NtiWarningLetterConditions.Where(c => wl.Conditions.Any(wlc => c.Id == wlc.Id)).Select(c => NtiWarningLetterMappers.ToServiceModel(c)).ToArray();
				}
			}

			return wl;
		}
	}
}