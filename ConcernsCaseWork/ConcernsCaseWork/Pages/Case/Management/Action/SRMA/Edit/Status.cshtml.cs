using ConcernsCaseWork.API.Contracts.Enums.TrustFinancialForecast;
using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Helpers;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Models.Validatable;
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
	public class EditSRMAStatusPageModel : AbstractPageModel
	{
		private readonly ISRMAService _srmaService;
		private readonly ILogger<EditSRMAStatusPageModel> _logger;

		[BindProperty]
		public RadioButtonsUiComponent SRMAStatus { get; set; }

		[BindProperty(SupportsGet = true, Name = "caseUrn")] public int CaseId { get; set; }
		[BindProperty(SupportsGet = true, Name = "srmaId")] public int SrmaId { get; set; }

		public EditSRMAStatusPageModel(ISRMAService srmaService, ILogger<EditSRMAStatusPageModel> logger)
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
					SRMAStatus = BuildSrmaStatusComponent(SRMAStatus.SelectedId);
					return Page();
				}

				await _srmaService.SetStatus(SrmaId, (SRMAStatus)SRMAStatus.SelectedId);
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
			SRMAStatus = BuildSrmaStatusComponent((int)model.Status);
		}

		private static RadioButtonsUiComponent BuildSrmaStatusComponent(int? selectedId = null)
		=> new(ElementRootId: "srma-status", Name: nameof(SRMAStatus), "")
		{
			RadioItems = new SimpleRadioItem[]
			{
				new (Enums.SRMAStatus.TrustConsidering.Description(), (int)Enums.SRMAStatus.TrustConsidering),
				new (Enums.SRMAStatus.PreparingForDeployment.Description(), (int)Enums.SRMAStatus.PreparingForDeployment),
				new (Enums.SRMAStatus.Deployed.Description(), (int)Enums.SRMAStatus.Deployed),
			},
			SelectedId = selectedId
		};
	}
}