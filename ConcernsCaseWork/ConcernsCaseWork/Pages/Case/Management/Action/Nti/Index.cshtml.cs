using ConcernsCaseWork.Constants;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Nti;
using ConcernsCaseWork.Service.Permissions;
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
		private readonly INtiConditionsService _ntiConditionsService;
		private readonly ICasePermissionsService _casePermissionsService;
		private readonly ILogger<IndexPageModel> _logger;

		public NtiModel NtiModel { get; set; }

		public ICollection<NtiConditionDto> NtiConditions { get; private set; }

		[BindProperty(SupportsGet = true, Name = "urn")]
		public int CaseId { get; set; }

		[BindProperty(SupportsGet = true, Name = "ntiId")]
		public int NtiId { get; set; }

		public bool UserCanDelete { get; set; }


		public Hyperlink BackLink => BuildBackLinkFromHistory(fallbackUrl: PageRoutes.YourCaseworkHomePage, "Back to case");

		public IndexPageModel(INtiModelService ntiModelService,
			INtiConditionsService ntiConditionsService,
			ICasePermissionsService casePermissionsService,
			ILogger<IndexPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_ntiConditionsService = ntiConditionsService;
			_casePermissionsService = casePermissionsService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				NtiModel = await GetNTIModel();
				UserCanDelete = await _casePermissionsService.UserHasDeletePermissions(CaseId);
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
					NtiConditions = await _ntiConditionsService.GetAllConditionsAsync();
					nti.Conditions = NtiConditions.Where(c => nti.Conditions.Any(ntic => ntic.Id == c.Id))?.Select(c => NtiMappers.ToServiceModel(c)).ToArray();
				}
			}

			return nti;
		}
	}
}