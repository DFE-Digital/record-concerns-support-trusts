using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
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
	public class EditTerritoryPageModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditTerritoryPageModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

		[BindProperty(SupportsGet = true)]
		public string ReferrerUrl => $"/case/{CaseUrn}/management";
		
		[BindProperty(SupportsGet = true, Name="Urn")]
		public int CaseUrn { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent Territory { get; set; }

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
				var caseModel = await _caseModelService.GetCaseByUrn((long)CaseUrn);
				LoadPageComponents(caseModel);
				
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
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
					LoadPageComponents();
					return Page();	
				}
				
				var userName = GetUserName();
				await _caseModelService.PatchTerritory((int)CaseUrn, userName, (Territory)Territory.SelectedId);
					
				return Redirect($"/case/{CaseUrn}/management");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}
		
		private string GetUserName() => _claimsPrincipalHelper.GetPrincipalName(User);

		private void LoadPageComponents(CaseModel model)
		{
			LoadPageComponents();

			Territory.SelectedId = model.Territory.HasValue ? (int)model.Territory.Value : null;
		}

		public void LoadPageComponents()
		{
			Territory = CaseComponentBuilder.BuildTerritory(nameof(Territory), Territory?.SelectedId);
			Territory.Heading = "";
		}
	}
}