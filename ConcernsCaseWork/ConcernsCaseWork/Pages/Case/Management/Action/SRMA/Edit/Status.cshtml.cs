using ConcernsCaseWork.API.Contracts.Srma;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Services.Cases;
using ConcernsCaseWork.Utils.Extensions;
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
		{
			var enumValues = new List<SRMAStatus>()
			{
				API.Contracts.Srma.SRMAStatus.TrustConsidering,
				API.Contracts.Srma.SRMAStatus.PreparingForDeployment,
				API.Contracts.Srma.SRMAStatus.Deployed
			};

			var radioItems = enumValues.Select(v =>
			{
				return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.ToString() };
			}).ToArray();

			return new(ElementRootId: "srma-status", Name: nameof(SRMAStatus), "")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				DisplayName = "SRMA status",
				Required = true
			};
		}
	}
}