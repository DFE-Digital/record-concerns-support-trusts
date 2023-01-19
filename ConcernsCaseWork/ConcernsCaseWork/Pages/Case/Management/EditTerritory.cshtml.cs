using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditTerritoryPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditTerritoryPageModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

		[BindProperty(SupportsGet = true)]
		public string ReferrerUrl => $"/case/{CaseUrn}/management";
		
		[BindProperty(SupportsGet = true, Name="Urn")]
		[Required(ErrorMessage = "CaseUrn is null or invalid to parse")]
		public int? CaseUrn { get; set; }
		
		[BindProperty(Name="territory")]
		[Required(ErrorMessage = "An SFSO territory must be selected")]
		public Territory? Territory { get; set; }
		
		public EditTerritoryPageModel(ICaseModelService caseModelService, IClaimsPrincipalHelper claimsPrincipalHelper, ILogger<EditTerritoryPageModel> logger)
		{
			_caseModelService = caseModelService;
			_claimsPrincipalHelper = claimsPrincipalHelper;
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
				Territory = caseModel.Territory;
				TempData["SelectedTerritory"] = Territory.ToString();
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
				await _caseModelService.PatchTerritory((int)CaseUrn, userName, Territory);
					
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