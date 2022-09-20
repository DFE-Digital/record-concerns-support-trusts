using ConcernsCaseWork.Pages.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ConcernsCaseWork.Models.CaseActions;
using System.Linq;
using System.Collections.Generic;
using ConcernsCaseWork.Mappers;
using ConcernsCasework.Service.Nti;
using ConcernsCaseWork.Services.Nti;
using Service.Redis.Nti;

namespace ConcernsCaseWork.Pages.Case.Management.Action.Nti
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class IndexPageModel : AbstractPageModel
	{
		private readonly INtiModelService _ntiModelService;
		private readonly INtiReasonsCachedService _ntiReasonsCachedService;
		private readonly INtiStatusesCachedService _ntiStatusesCachedService;
		private readonly INtiConditionsCachedService _ntiConditionsCachedService;
		private readonly ILogger<IndexPageModel> _logger;

		public NtiModel NtiModel { get; set; }

		public ICollection<NtiReasonDto> NtiReasons { get; private set; }
		public ICollection<NtiStatusDto> NtiStatuses { get; set; }
		public ICollection<NtiConditionDto> NtiConditions { get; private set; }

		public IndexPageModel(INtiModelService ntiModelService,
			INtiReasonsCachedService ntiReasonsCachedService,
			INtiStatusesCachedService ntiStatusesCachedService,
			INtiConditionsCachedService ntiConditionsCachedService,
			ILogger<IndexPageModel> logger)
		{
			_ntiModelService = ntiModelService;
			_ntiReasonsCachedService = ntiReasonsCachedService;
			_ntiStatusesCachedService = ntiStatusesCachedService;
			_ntiConditionsCachedService = ntiConditionsCachedService;
			_logger = logger;
		}

		public async Task OnGetAsync()
		{
			long caseUrn = 0;
			long ntiId = 0;

			try
			{
				_logger.LogInformation("Case::Action::NTI::IndexPageModel::OnGetAsync");

				
				(caseUrn, ntiId) = GetRouteData();

				NtiModel = await GetNTIModel(ntiId);

				if (NtiModel == null)
				{
					throw new Exception($"Could not load NTI with ID {ntiId}");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::NTI::IndexPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		private async Task<NtiModel> GetNTIModel(long ntiId)
		{
			var nti = await _ntiModelService.GetNtiByIdAsync(ntiId);

			if (nti != null)
			{
				if (nti.Reasons?.Any() == true)
				{
					NtiReasons = await _ntiReasonsCachedService.GetAllReasonsAsync();
					nti.Reasons = NtiReasons.Where(r => nti.Reasons.Any(ntir => ntir.Id == r.Id))?.Select(r => NtiMappers.ToServiceModel(r)).ToArray();
				}

				if (nti.Conditions?.Any() == true)
				{
					NtiConditions = await _ntiConditionsCachedService.GetAllConditionsAsync();
					nti.Conditions = NtiConditions.Where(c => nti.Conditions.Any(ntic => ntic.Id == c.Id))?.Select(c => NtiMappers.ToServiceModel(c)).ToArray();
				}
			}

			return nti;
		}

		private (long caseUrn, long ntiId) GetRouteData()
		{
			var caseUrnValue = RouteData.Values["urn"];
			if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out long caseUrn) || caseUrn == 0)
				throw new Exception("CaseUrn is null or invalid to parse");

			var ntiIdValue = RouteData.Values["ntiId"];
			if (ntiIdValue == null || !long.TryParse(ntiIdValue.ToString(), out long ntiId) || ntiId == 0)
				throw new Exception("Nti Id is null or invalid to parse");

			return (caseUrn, ntiId);
		}
	}
}