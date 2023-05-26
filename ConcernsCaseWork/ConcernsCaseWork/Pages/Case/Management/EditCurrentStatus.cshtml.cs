using Ardalis.GuardClauses;
using ConcernsCaseWork.CoreTypes;
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
		
		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int CaseUrn { get; set; }

		[BindProperty]
		public TextAreaUiComponent CurrentStatus { get; set; }

		public EditCurrentStatusPageModel(ICaseModelService caseModelService, ILogger<EditCurrentStatusPageModel> logger,
			TelemetryClient telemetryClient)
		{
			_caseModelService = Guard.Against.Null(caseModelService);
			_logger = Guard.Against.Null(logger);
			_telemetryClient = Guard.Against.Null(telemetryClient);
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				var model = await _caseModelService.GetCaseByUrn(CaseUrn);

				LoadPage(model);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}
		
		public async Task<ActionResult> OnPostEditCurrentStatus()
		{
			try
			{
				_logger.LogInformation("Case::EditCurrentStatusPageModel::OnPostEditCurrentStatus");
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = CaseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					CurrentStatus = CurrentStatus.Text.StringContents
				};
				AppInsightsHelper.LogEvent(_telemetryClient, new AppInsightsModel()
				{
					EventName = "EDIT current status",
					EventDescription = $"Current status has been changed {CaseUrn}",
					EventPayloadJson = JsonSerializer.Serialize(patchCaseModel),
					EventUserName = User.Identity.Name
				});
				await _caseModelService.PatchCurrentStatus(patchCaseModel);

				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPage(CaseModel model)
		{
			LoadPage();

			CurrentStatus.Text.StringContents = model.CurrentStatus;
		}

		private void LoadPage()
		{
			CurrentStatus = CaseComponentBuilder.BuildCurrentStatus(nameof(CurrentStatus), CurrentStatus?.Text.StringContents);
			CurrentStatus.Heading = "";
		}
	}
}