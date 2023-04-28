using Ardalis.GuardClauses;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditCaseHistoryPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditCaseHistoryPageModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;
		private TelemetryClient _telemetryClient;

		[BindProperty(SupportsGet = true)]
		public string ReferrerUrl => $"/case/{CaseUrn}/management";
		
		[BindProperty(SupportsGet = true, Name="Urn")]
		[Required(ErrorMessage = "CaseUrn is null or invalid to parse")]
		public long? CaseUrn { get; set; }
		
		[BindProperty(Name="case-history")]
		[MaxLength(4300, ErrorMessage = "Case history must be 4300 characters or less")]
		public string CaseHistory { get; set; }
		
		public EditCaseHistoryPageModel(ICaseModelService caseModelService, IClaimsPrincipalHelper claimsPrincipalHelper,
			ILogger<EditCaseHistoryPageModel> logger)
		{
			_caseModelService = Guard.Against.Null(caseModelService);
			_claimsPrincipalHelper = Guard.Against.Null(claimsPrincipalHelper);
			_logger = logger;
			
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			_logger.LogMethodEntered();

			try
			{
				if (!ModelState.IsValid)
				{
					return Page();
				}
				
				var caseModel = await _caseModelService.GetCaseByUrn((long)CaseUrn);
				CaseHistory = caseModel.CaseHistory;
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return Page();
		}
		
		public async Task<ActionResult> OnPost()
		{
			_logger.LogMethodEntered();
			
			try
			{
				if (!ModelState.IsValid)
				{
					return Page();	
				}
				
				var userName = GetUserName();
				await _caseModelService.PatchCaseHistory((long)CaseUrn, userName, CaseHistory);
				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return Page();
		}
		
		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);
	}
}