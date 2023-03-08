using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Constants;
using System;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast
{
	public class IndexPageModel : AbstractPageModel
	{
		private ITrustFinancialForecastService _trustFinancialForecastService;
		private ICasePermissionsService _casePermissionsService;
		private ILogger<IndexPageModel> _logger;
		
		[BindProperty(SupportsGet = true)] public int Urn { get; init; }
		[BindProperty(SupportsGet = true, Name="Id")] public int TrustFinancialForecastId { get; init; }

		public ViewTrustFinancialForecastModel TrustFinancialForecastModel { get; set; }

		public IndexPageModel(
			ITrustFinancialForecastService trustFinancialForecastService,
			ICasePermissionsService casePermissionsService,
			ILogger<IndexPageModel> logger)
		{
			_trustFinancialForecastService = trustFinancialForecastService;
			_casePermissionsService = casePermissionsService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				var request = new GetTrustFinancialForecastByIdRequest { CaseUrn = Urn, TrustFinancialForecastId = TrustFinancialForecastId };

				var trustFinancialForecast = await _trustFinancialForecastService.GetById(request);

				var casePermissions = await _casePermissionsService.GetCasePermissions(Urn);

				TrustFinancialForecastModel = TrustFinancialForecastMapping.ToViewModel(trustFinancialForecast, casePermissions);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::Action::TrustFinancialForecast::IndexPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				SetErrorMessage(ErrorConstants.ErrorOnGetPage);
			}

			return Page();
		}
	}
}
