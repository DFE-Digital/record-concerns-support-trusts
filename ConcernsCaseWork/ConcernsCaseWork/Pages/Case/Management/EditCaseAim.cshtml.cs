using Ardalis.GuardClauses;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Services.Cases;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditCaseAimPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditCaseAimPageModel> _logger;
		private TelemetryClient _telemetryClient;
		
		
		
		public CaseModel CaseModel { get; private set; }
		
		public EditCaseAimPageModel(ICaseModelService caseModelService, ILogger<EditCaseAimPageModel> logger,
			TelemetryClient telemetryClient)
		{
			_caseModelService = Guard.Against.Null(caseModelService);
			_logger = Guard.Against.Null(logger);
			_telemetryClient = Guard.Against.Null(telemetryClient);
			

		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditCaseAimPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCaseAimPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}
		
		public async Task<ActionResult> OnPostEditCaseAim(string url)
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditCaseAimPageModel::OnPostEditCaseAim");
				
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");

				var caseAim = Request.Form["case-aim"];
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					CaseAim = caseAim
				};
				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "CASE CLOSED",
					EventDescription = $"Case has been closed {caseUrn}",
					EventPayloadJson = JsonSerializer.Serialize(patchCaseModel),
					EventUserName = User.Identity.Name
				});
				await _caseModelService.PatchCaseAim(patchCaseModel);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCaseAimPageModel::OnPostEditCaseAim::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url, caseUrn);
		}
		
		private async Task<ActionResult> LoadPage(string url, long caseUrn)
		{
			try
			{
				if (caseUrn == 0)
					throw new Exception("Case urn cannot be 0");
				
				CaseModel = await _caseModelService.GetCaseByUrn(caseUrn);
				CaseModel.PreviousUrl = url;
				
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCaseAimPageModel::LoadPage::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}
		
		
	}
}