using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Authorization;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management
{
	public class EditRegionModel : AbstractPageModel
	{
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditRegionModel> _logger;
		private readonly IClaimsPrincipalHelper _claimsPrincipalHelper;

		[BindProperty(SupportsGet = true)]
		public string ReferrerUrl => $"/case/{CaseUrn}/management";

		[BindProperty(SupportsGet = true, Name = "Urn")]
		public int CaseUrn { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent Region { get; set; }

		public EditRegionModel(ICaseModelService caseModelService, IClaimsPrincipalHelper claimsPrincipalHelper, ILogger<EditRegionModel> logger)
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

				await _caseModelService.PatchRegion((int)CaseUrn, (Region)Region.SelectedId);

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

			Region.SelectedId = model.Region.HasValue ? (int)model.Region.Value : null;
		}

		public void LoadPageComponents()
		{
			Region = CaseComponentBuilder.BuildRegion(nameof(Region), Region?.SelectedId);
			Region.Heading = "";
		}
	}
}
