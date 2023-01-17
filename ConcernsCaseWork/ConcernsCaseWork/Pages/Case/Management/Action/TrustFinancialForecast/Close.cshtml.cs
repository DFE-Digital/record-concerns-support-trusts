using ConcernsCaseWork.API.Contracts.Constants;
using ConcernsCaseWork.API.Contracts.RequestModels.TrustFinancialForecasts;
using ConcernsCaseWork.API.Contracts.ResponseModels.TrustFinancialForecasts;
using ConcernsCaseWork.Exceptions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Service.TrustFinancialForecast;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class ClosePageModel : AbstractPageModel
	{
		private readonly ITrustFinancialForecastService _trustFinancialForecastService;
		private readonly ILogger<ClosePageModel> _logger;
		
		[BindProperty(SupportsGet = true)]
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "TrustFinancialForecastId must be provided")]
		public int TrustFinancialForecastId { get; set; }

		[BindProperty(Name="Urn", SupportsGet = true)]
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "CaseUrn must be provided")]
		public int CaseUrn { get; set; }
		
		[BindProperty]
		public TextAreaUiComponent Notes { get; set; } = BuildNotesComponent();

		public ClosePageModel(ITrustFinancialForecastService trustFinancialForecastService, ILogger<ClosePageModel> logger)
		{
			_trustFinancialForecastService = trustFinancialForecastService;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					SetErrorMessage(ErrorOnGetPage);
					return Page();
				}

				var request = new GetTrustFinancialForecastByIdRequest{ CaseUrn = CaseUrn, TrustFinancialForecastId = TrustFinancialForecastId };
				if (!request.IsValid())
				{
					SetErrorMessage(ErrorOnGetPage);
					return Page();
				}
				
				var trustFinancialForecast = await _trustFinancialForecastService.GetById(request);

				if (IsClosed(trustFinancialForecast))
				{
					return Redirect($"/case/{CaseUrn}/management/action/trustfinancialforecast/{TrustFinancialForecastId}/closed");
				}
				
				if (!IsEditable(trustFinancialForecast))
				{
					return Redirect($"/case/{CaseUrn}/management/action/trustFinancialForecast/{TrustFinancialForecastId}");
				}
				
				Notes = BuildNotesComponent(trustFinancialForecast.Notes);
				
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnGetPage);

				return Page();
			}
		}

		public async Task<IActionResult> OnPostAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					return Page();
				}

				var closeTrustFinancialForecastRequest = new CloseTrustFinancialForecastRequest
				{
					CaseUrn = CaseUrn,
					TrustFinancialForecastId = TrustFinancialForecastId,
					Notes = Notes.Contents
				};

				if (!closeTrustFinancialForecastRequest.IsValid())
				{
					return Page();
				}

				await _trustFinancialForecastService.Close(closeTrustFinancialForecastRequest);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (InvalidUserInputException ex)
			{
				TempData["TrustFinancialForecast.Message"] = new List<string>() { ex.Message };
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				SetErrorMessage(ErrorOnPostPage);
			}
			return Page();
		}

		private static bool IsEditable(TrustFinancialForecastResponse trustFinancialForecast) => !IsClosed(trustFinancialForecast);
		private static bool IsClosed(TrustFinancialForecastResponse trustFinancialForecast) => trustFinancialForecast.ClosedAt.HasValue;
		
		private static TextAreaUiComponent BuildNotesComponent(string contents = "")
			=> new("notes", nameof(Notes), "Finalise notes")
			{
				MaxLength = TrustFinancialForecastConstants.MaxNotesLength,
				Contents = contents
			};
	}
}