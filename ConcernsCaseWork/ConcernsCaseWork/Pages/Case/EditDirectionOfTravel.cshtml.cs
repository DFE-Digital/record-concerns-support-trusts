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
	public class EditDirectionOfTravelPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditDirectionOfTravelPageModel> _logger;
		
		public string PreviousUrl { get; private set; }

		public EditDirectionOfTravelPageModel(ICaseModelService caseModelService, ILogger<EditDirectionOfTravelPageModel> logger)
		{
			_caseModelService = caseModelService;
			_logger = logger;
		}
		
		public ActionResult OnGet()
		{
			_logger.LogInformation("EditDirectionOfTravelPageModel::OnGet");

			return LoadPage(Request.Headers["Referer"].ToString());
		}
		
		public async Task<ActionResult> OnPostEditDirectionOfTravel(string url)
		{
			try
			{
				_logger.LogInformation("EditDirectionOfTravelPageModel::OnPostEditDirectionOfTravel");
				
				var caseUrnValue = RouteData.Values["id"];
				if (caseUrnValue == null || !long.TryParse(caseUrnValue.ToString(), out var caseUrn))
				{
					throw new Exception("EditDirectionOfTravelPageModel::CaseUrn is null or invalid to parse");
				}
				
				var directionOfTravel = Request.Form["direction-of-travel"];

				if (string.IsNullOrEmpty(directionOfTravel)) throw new Exception("Missing form values");
				
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
				_logger.LogError($"Case::EditDirectionOfTravelPageModel::OnPostEditDirectionOfTravel::Exception - {ex.Message}");

				TempData["Error.Message"] = ErrorOnPostPage;
			}

			return LoadPage(url);
		}
		
		private ActionResult LoadPage(string url)
		{
			PreviousUrl = url;

			return Page();
		}
	}
}