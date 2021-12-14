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
	public class EditDirectionOfTravelPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditDirectionOfTravelPageModel> _logger;
		
		public CaseModel CaseModel { get; private set; }
		
		public EditDirectionOfTravelPageModel(ICaseModelService caseModelService, ILogger<EditDirectionOfTravelPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditDirectionOfTravelPageModel::OnGetAsync");

				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
				{
					throw new Exception("Case::EditDirectionOfTravelPageModel::CaseUrn is null or invalid to parse");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditDirectionOfTravelPageModel::OnGetAsync::Exception - {Message}", ex.Message);
				
				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), caseUrn);
		}
		
		public async Task<ActionResult> OnPostEditDirectionOfTravel(string url)
		{
			long caseUrn = 0;
			
			try
			{
				_logger.LogInformation("Case::EditDirectionOfTravelPageModel::OnPostEditDirectionOfTravel");
				
				var caseUrnValue = RouteData.Values["urn"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out caseUrn) || caseUrn == 0)
				{
					throw new Exception("Case::EditDirectionOfTravelPageModel::CaseUrn is null or invalid to parse");
				}
				
				var directionOfTravel = Request.Form["direction-of-travel"];

				if (string.IsNullOrEmpty(directionOfTravel)) 
					throw new Exception("Case::EditDirectionOfTravelPageModel::Missing form values");
				
				// Create patch case model
				var patchCaseModel = new PatchCaseModel
				{
					Urn = caseUrn,
					CreatedBy = User.Identity.Name,
					UpdatedAt = DateTimeOffset.Now,
					DirectionOfTravel = directionOfTravel
				};
					
				await _caseModelService.PatchDirectionOfTravel(patchCaseModel);
					
				return Redirect(url);
			}
			catch (Exception ex)
			{
				_logger.LogError("Case::EditDirectionOfTravelPageModel::OnPostEditDirectionOfTravel::Exception - {Message}", ex.Message);

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