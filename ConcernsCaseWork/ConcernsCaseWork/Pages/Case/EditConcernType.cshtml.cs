using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Services.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

		public IDictionary<string, IList<string>> TypesDictionary { get; private set; }
		public string PreviousUrl { get; private set; }
		
		public EditConcernTypePageModel(ITypeModelService typeModelService, ICaseModelService caseModelService, 
			ILogger<EditConcernTypePageModel> logger)
		{
			_typeModelService = typeModelService;
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			_logger.LogInformation("EditConcernTypePageModel::OnGetAsync");

			return await LoadPage(Request.Headers["Referer"].ToString());
		}
		
		public async Task<ActionResult> OnPostEditConcernType(string url)
		{
			try
			{
				_logger.LogInformation("EditConcernTypePageModel::OnPostEditConcernType");
				
				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn))
				{
					throw new Exception("ManagementPageModel::CaseUrn is null or invalid to parse");
				}
				
				var type = Request.Form["type"];
				var subType = Request.Form["subType"];

				if (!string.IsNullOrEmpty(type))
				{
					// Create update case model
					var patchCaseModel = new PatchCaseModel
					{
						Urn = caseUrn,
						CreatedBy = User.Identity.Name,
						UpdatedAt = DateTimeOffset.Now,
						RecordType = type,
						RecordSubType = subType
					};
					
					await _caseModelService.UpdateCase(patchCaseModel);
					
					return Redirect(url);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Case::ManagementPageModel::OnGetAsync::Exception - {ex.Message}");

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return await LoadPage(url);
		}

		private async Task<ActionResult> LoadPage(string url)
		{
			TypesDictionary = await _typeModelService.GetTypes();
			PreviousUrl = url;

			return Page();
		} 
	}
}