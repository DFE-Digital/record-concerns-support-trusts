// using ConcernsCaseWork.API.Contracts.Constants;
// using ConcernsCaseWork.API.Contracts.RequestModels.Concerns.TrustFinancialForecasts;
// using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.TrustFinancialForecasts;
// using ConcernsCaseWork.Exceptions;
// using ConcernsCaseWork.Logging;
// using ConcernsCaseWork.Pages.Base;
// using ConcernsCaseWork.Service.TrustFinancialForecast;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.Threading.Tasks;
//
// namespace ConcernsCaseWork.Pages.Case.Management.Action.TrustFinancialForecast
// {
// 	[Authorize]
// 	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
// 	public class ClosePageModel : AbstractPageModel
// 	{
// 		private readonly ITrustFinancialForecastService _trustFinancialForecastService;
// 		private readonly ILogger<ClosePageModel> _logger;
// 		
// 		public int NotesMaxLength => TrustFinancialForecastConstants.MaxSupportingNotesLength;
// 		
// 		[BindProperty(SupportsGet = true)]
// 		[Required]
// 		[Range(1, int.MaxValue, ErrorMessage = "TrustFinancialForecastId must be provided")]
// 		public int TrustFinancialForecastId { get; set; }
//
// 		[BindProperty(Name="Urn", SupportsGet = true)]
// 		[Required]
// 		[Range(1, int.MaxValue, ErrorMessage = "CaseUrn must be provided")]
// 		public int CaseUrn { get; set; }
// 		
// 		[BindProperty(Name="SupportingNotes")]
// 		[MaxLength(TrustFinancialForecastConstants.MaxSupportingNotesLength, ErrorMessage = "Supporting Notes must be 2000 characters or less")]
// 		public string Notes { get; set; }
//
// 		public ClosePageModel(ITrustFinancialForecastService trustFinancialForecastService, ILogger<ClosePageModel> logger)
// 		{
// 			_trustFinancialForecastService = trustFinancialForecastService;
// 			_logger = logger;
// 		}
//
// 		public async Task<IActionResult> OnGetAsync()
// 		{
// 			_logger.LogMethodEntered();
//
// 			try
// 			{
// 				if (!ModelState.IsValid)
// 				{
// 					return Page();
// 				}
// 				
// 				var trustFinancialForecast = await _trustFinancialForecastService.GetTrustFinancialForecast(CaseUrn, TrustFinancialForecastId);
//
// 				if (!IsTrustFinancialForecastEditable(trustFinancialForecast))
// 				{
// 					return Redirect($"/case/{CaseUrn}/management/action/trustFinancialForecast/{TrustFinancialForecastId}");
// 				}
// 				
// 				Notes = trustFinancialForecast.SupportingNotes;
// 				
// 				return Page();
// 			}
// 			catch (Exception ex)
// 			{
// 				_logger.LogErrorMsg(ex);
//
// 				SetErrorMessage(ErrorOnGetPage);
//
// 				return Page();
// 			}
// 		}
//
// 		public async Task<IActionResult> OnPostAsync()
// 		{
// 			_logger.LogMethodEntered();
//
// 			try
// 			{
// 				if (!ModelState.IsValid)
// 				{
// 					return Page();
// 				}
//
// 				var updateTrustFinancialForecastRequest = new CloseTrustFinancialForecastRequest { SupportingNotes = Notes };
//
// 				await _trustFinancialForecastService.CloseTrustFinancialForecast(CaseUrn, TrustFinancialForecastId, updateTrustFinancialForecastRequest);
//
// 				return Redirect($"/case/{CaseUrn}/management");
// 			}
// 			catch (InvalidUserInputException ex)
// 			{
// 				TempData["TrustFinancialForecast.Message"] = new List<string>() { ex.Message };
// 			}
// 			catch (Exception ex)
// 			{
// 				_logger.LogErrorMsg(ex);
//
// 				SetErrorMessage(ErrorOnPostPage);
// 			}
// 			return Page();
// 		}
//
// 		// TODO: These ought to be part of a general service / model
// 		private static bool IsTrustFinancialForecastEditable(GetTrustFinancialForecastResponse trustFinancialForecast) => !IsTrustFinancialForecastClosed(trustFinancialForecast) && TrustFinancialForecastHasOutcome(trustFinancialForecast);
// 		private static bool IsTrustFinancialForecastClosed(GetTrustFinancialForecastResponse trustFinancialForecast) => trustFinancialForecast.ClosedAt.HasValue;
// 		private static bool TrustFinancialForecastHasOutcome(GetTrustFinancialForecastResponse trustFinancialForecast) => trustFinancialForecast.Outcome != null;
// 	}
// }