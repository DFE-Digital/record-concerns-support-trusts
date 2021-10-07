using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditConcernTypePageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ILogger<EditConcernTypePageModel> _logger;

		public CaseModel CaseModel { get; private set; }
		
		public EditConcernTypePageModel(ITypeModelService typeModelService, ICaseModelService caseModelService, 
			ILogger<EditConcernTypePageModel> logger)
		{
			_typeModelService = typeModelService;
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditConcernTypePageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditConcernTypePageModel::CaseUrn is null or invalid to parse");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditConcernTypePageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}
		
		public async Task<ActionResult> OnPostEditConcernType(string url)
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditConcernTypePageModel::OnPostEditConcernType");
				
				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn))
				{
					throw new Exception("Case::EditConcernTypePageModel::CaseUrn is null or invalid to parse");
				}
				
				var type = Request.Form["type"];
				var subType = Request.Form["subType"];

				if (!IsValidEditConcernType(type, ref subType)) 
					throw new Exception("Case::EditConcernTypePageModel::Missing form values");
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					CaseType = type,
					CaseSubType = subType
				};
					
				await _caseModelService.PatchConcernType(patchCaseModel);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditConcernTypePageModel::OnPostEditConcernType::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url, caseUrn);
		}

		private async Task<ActionResult> LoadPage(string url, long caseUrn)
		{
			if (caseUrn != 0) {
				CaseModel = await _caseModelService.GetCaseByUrn(User.Identity.Name, caseUrn);
			}
			else
			{
				CaseModel = new CaseModel();
			}

			CaseModel.TypesDictionary = await _typeModelService.GetTypes();
			CaseModel.PreviousUrl = url;
			
			return Page();
		}
	}
}