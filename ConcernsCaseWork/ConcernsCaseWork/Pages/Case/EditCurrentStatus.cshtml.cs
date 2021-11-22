using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditCurrentStatusPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditCurrentStatusPageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		
		public EditCurrentStatusPageModel(ICaseModelService caseModelService, ILogger<EditCurrentStatusPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditCurrentStatusPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditCurrentStatusPageModel::CaseUrn is null or invalid to parse");
				}
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
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditCurrentStatusPageModel::CaseUrn is null or invalid to parse");
				}
				
				var currentStatus = Request.Form["current-status"];
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					CurrentStatus = currentStatus
				};

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
			if (caseUrn == 0) return Page();
				
			CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			CaseModel.PreviousUrl = url;
			
			return Page();
		}
	}
}