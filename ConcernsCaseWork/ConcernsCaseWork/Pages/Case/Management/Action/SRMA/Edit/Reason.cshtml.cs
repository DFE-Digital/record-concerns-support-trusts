using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Case.Management.Action.SRMA.Edit
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditSRMAReasonOfferedPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaService;
		private readonly ICaseModelService _caseModelService;
		private readonly ILogger<EditSRMAReasonOfferedPageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "caseUrn")] public int CaseId { get; set; }
		[BindProperty(SupportsGet = true, Name = "srmaId")] public int SrmaId { get; set; }

		[BindProperty]
		public Division? Division { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent SRMAReasonOffered { get; set; }

		public EditSRMAReasonOfferedPageModel(
			ISRMAService srmaService, 
			ICaseModelService caseModelService, 
			ILogger<EditSRMAReasonOfferedPageModel> logger)
		{
			_srmaService = srmaService;
			_caseModelService = caseModelService;
			_logger = logger;
		}

		public async Task<ActionResult> OnGetAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				var model = await _srmaService.GetSRMAById(SrmaId);
					
				if (model.IsClosed)
				{
					return Redirect($"/case/{CaseId}/management/action/srma/{SrmaId}/closed");
				}

				var caseModel = await _caseModelService.GetCaseByUrn(CaseId);

				Division = caseModel.Division;

				LoadPageComponents(model);
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnGetPage);
			}

			return Page();
		}

		public async Task<ActionResult> OnPostAsync()
		{
			try
			{
				_logger.LogMethodEntered();

				if (!ModelState.IsValid)
				{
					SRMAReasonOffered = BuildSrmaReasonComponent(SRMAReasonOffered.SelectedId);
					return Page();
				}

				await _srmaService.SetReason(SrmaId, (SRMAReasonOffered)SRMAReasonOffered.SelectedId);
				return Redirect($"/case/{CaseId}/management/action/srma/{SrmaId}");
			}
			catch (Exception ex)
			{
				_logger.LogErrorMsg(ex);
				SetErrorMessage(ErrorOnPostPage);
			}

			return Page();
		}

		private void LoadPageComponents(SRMAModel model)
		{
			SRMAReasonOffered = BuildSrmaReasonComponent((int)model.Reason);
		}

		private static RadioButtonsUiComponent BuildSrmaReasonComponent(int? selectedId = null)
		{
			var enumValues = new List<SRMAReasonOffered>()
			{
				API.Contracts.Srma.SRMAReasonOffered.OfferLinked,
				API.Contracts.Srma.SRMAReasonOffered.SchoolsFinancialSupportAndOversight,
				API.Contracts.Srma.SRMAReasonOffered.RegionsGroupIntervention
			};

			var radioItems = enumValues.Select(v =>
			{
				return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.ToString() };
			}).ToArray();

			return new(ElementRootId: "srma-reason-offered", Name: nameof(SRMAReasonOffered), "")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "SRMA reason"
			};
		} 
	}
}