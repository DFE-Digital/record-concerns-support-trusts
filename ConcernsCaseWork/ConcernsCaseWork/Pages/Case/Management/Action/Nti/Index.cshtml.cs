using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Nti;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Services.Nti;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;
		private readonly INtiConditionsCachedService _ntiConditionsCachedService;
		private readonly ILogger<IndexPageModel> _logger;

		public NtiModel NtiModel { get; set; }

		public ICollection<NtiConditionDto> NtiConditions { get; private set; }

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseId { get; set; }

		[BindProperty(SupportsGet = true, Name = "ntiId")]
		public int NtiId { get; set; }
		
		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case");

		public IndexPageModel(INtiModelService ntiModelService,
			INtiConditionsCachedService ntiConditionsCachedService,
			ILogger<IndexPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_ntiConditionsCachedService = ntiConditionsCachedService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				NtiModel = await GetNTIModel();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}
		}

		private async Task<NtiModel> GetNTIModel()
		{
			var nti = await _ntiModelService.GetNtiViewModelAsync(CaseId, NtiId);

			if (nti != null)
			{
				if (nti.Conditions?.Any() == true)
				{
					NtiConditions = await _ntiConditionsCachedService.GetAllConditionsAsync();
					nti.Conditions = NtiConditions.Where(c => nti.Conditions.Any(ntic => ntic.Id == c.Id))?.Select(c => NtiMappers.ToServiceModel(c)).ToArray();
				}
			}

			return nti;
		}
	}
}