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
		private readonly ILogger<EditSRMAReasonOfferedPageModel> _logger;

		[BindProperty(SupportsGet = true, Name = "caseUrn")] public int CaseId { get; set; }
		[BindProperty(SupportsGet = true, Name = "srmaId")] public int SrmaId { get; set; }

		[BindProperty]
		public RadioButtonsUiComponent SRMAReasonOffered { get; set; }

		public EditSRMAReasonOfferedPageModel(ISRMAService srmaService, ILogger<EditSRMAReasonOfferedPageModel> logger)
		{
			this._srmaService = srmaService;
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
				Enums.SRMAReasonOffered.OfferLinked,
				Enums.SRMAReasonOffered.SchoolsFinancialSupportAndOversight,
				Enums.SRMAReasonOffered.RegionsGroupIntervention
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
				DisplayName = "Reason"
			};
		} 
	}
}