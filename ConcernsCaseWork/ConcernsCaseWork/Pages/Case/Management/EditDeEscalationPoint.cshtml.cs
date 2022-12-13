using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditDeEscalationPointPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditDeEscalationPointPageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		
		public EditDeEscalationPointPageModel(ICaseModelService caseModelService, ILogger<EditDeEscalationPointPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditDeEscalationPointPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditDeEscalationPointPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}
		
		public async Task<ActionResult> OnPostEditDeEscalationPoint(string url)
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditDeEscalationPointPageModel::OnPostEditDeEscalationPoint");
				
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
					throw new Exception("CaseUrn is null or invalid to parse");

				var deEscalationPoint = Request.Form["de-escalation-point"];
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					DeEscalationPoint = deEscalationPoint
				};

				await _caseModelService.PatchDeEscalationPoint(patchCaseModel);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditDeEscalationPointPageModel::OnPostEditDeEscalationPoint::Exception - {Message}", ex.Message);

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
				_logger.LogError("Case::EditDeEscalationPointPageModel::LoadPage::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
				return Page();
			}
		}
	}
}