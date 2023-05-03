using Ardalis.GuardClauses;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
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
	public class EditCurrentStatusPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditCurrentStatusPageModel> _logger;
		private TelemetryClient _telemetryClient;
		
		public CaseModel CaseModel { get; private set; }
		
		public EditCurrentStatusPageModel(ICaseModelService caseModelService, ILogger<EditCurrentStatusPageModel> logger,
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
				_logger.LogInformation("Case::EditCurrentStatusPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCurrentStatusPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}
		
		public async Task<ActionResult> OnPostEditCurrentStatus(string url)
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditCurrentStatusPageModel::OnPostEditCurrentStatus");
				
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");

				var currentStatus = Request.Form["current-status"];
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					CurrentStatus = currentStatus
				};
				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "EDIT case aim",
					EventDescription = $"Case aim has been changed {caseUrn}",
					EventPayloadJson = JsonSerializer.Serialize(patchCaseModel),
					EventUserName = User.Identity.Name
				});
				await _caseModelService.PatchCurrentStatus(patchCaseModel);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditCurrentStatusPageModel::OnPostEditCurrentStatus::Exception - {Message}", ex.Message);

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
				_logger.LogError("Case::EditCurrentStatusPageModel::LoadPage::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}
	}
}